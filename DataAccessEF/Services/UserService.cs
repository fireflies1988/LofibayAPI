using AutoMapper;
using Common.Helpers;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests;
using Domain.Models.DTOs.Responses;
using Domain.Models.ResponseTypes;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DataAccessEF.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public int GetCurrentUserId()
        {
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public string GetCurrentEmail()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
        }

        public string GetCurrentUsername()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }

        public async Task<BaseResponse<TokenResponse>> LoginAsync(LoginRequest loginRequest)
        {
            User? existingUser = (await _unitOfWork.Users.GetAsync(filter: u => u.Email == loginRequest.Email && !u.DeletedDate.HasValue, includeProperties: "RefreshTokens,Role")).SingleOrDefault();
            if (existingUser == null)
            {
                return new FailResponse<TokenResponse>
                {
                    Message = "Incorrect email or password."
                };
            }

            // verify password hash
            string loginPasswordHash = PasswordHelper.HashUsingPbkdf2(loginRequest.Password!, Convert.FromBase64String(existingUser.PasswordSalt!));
            if (existingUser.PasswordHash != loginPasswordHash)
            {
                return new FailResponse<TokenResponse>
                {
                    Message = "Incorrect email or password."
                };
            }

            Tuple<string, string>? token = await _tokenService.GenerateTokensAsync(existingUser);
            return new SuccessResponse<TokenResponse>
            {
                Data = new TokenResponse
                {
                    AccessToken = token?.Item1,
                    RefreshToken = token?.Item2
                }
            };
        }

        public async Task<BaseResponse<LogoutResponse>> LogoutAsync()
        {
            RefreshToken? refreshToken = await _unitOfWork.RefreshTokens.GetFirstOrDefaultAsync(filter: rt => rt.UserId == GetCurrentUserId());

            if (refreshToken == null)
            {
                return new SuccessResponse<LogoutResponse>();
            }

            _unitOfWork.RefreshTokens?.Remove(refreshToken);
            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                return new SuccessResponse<LogoutResponse>();
            }

            return new FailResponse<LogoutResponse>
            {
                Message = "Unable to logout user."
            };
        }

        public async Task<BaseResponse<SignupResponse>> SignupAsync(SignupRequest signupRequest)
        {
            User? existingUserWithThisEmail = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Email == signupRequest.Email && !u.DeletedDate.HasValue);
            if (existingUserWithThisEmail != null)
            {
                return new FailResponse<SignupResponse>
                {
                    Message = "An account already exists with the same email."
                };
            }

            User? existingUserWithThisUsername = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Username == signupRequest.Username && !u.DeletedDate.HasValue);
            if (existingUserWithThisUsername != null)
            {
                return new FailResponse<SignupResponse>
                {
                    Message = "A user with this username already exists."
                };
            }

            if (signupRequest.Password != signupRequest.ConfirmPassword)
            {
                return new FailResponse<SignupResponse>
                {
                    Message = "Password and confirm password do not match."
                };
            }

            // password requirements
            //if (signupRequest.Password?.Length <= 7)
            //{
            //    return new FailResponse<SignupResponse>
            //    {
            //        Message = "Your password must have at least 8 characters."
            //    };
            //}

            byte[] salt = PasswordHelper.GetSecureSalt();
            string passwordHash = PasswordHelper.HashUsingPbkdf2(signupRequest.Password!, salt);
            var user = new User
            {
                Email = signupRequest.Email,
                Username = signupRequest.Username,
                PasswordHash = passwordHash,
                PasswordSalt = Convert.ToBase64String(salt),
                FirstName = signupRequest.FirstName,
                LastName = signupRequest.LastName,
                // Active = true // You can save is false and send confirmation email 
                // to the user, then once the user confirms the email you can make it true
            };
            await _unitOfWork.Users.AddAsync(user);

            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                return new SuccessResponse<SignupResponse>
                {
                    Data = new SignupResponse
                    {
                        Email = user.Email
                    }
                };
            }

            return new FailResponse<SignupResponse>
            {
                Message = "Unable to save the user."
            };
        }

        public async Task<BaseResponse<UserInfoResponse>> UpdateUserAsync(UpdateUserRequest updateUserRequest)
        {
            User? currentUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(filter: u => u.UserId == GetCurrentUserId(), includeProperties: "Address");

            if (updateUserRequest.Email != null && updateUserRequest.Email != currentUser?.Email)
            {
                User? existingUserWithThisEmail = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Email == updateUserRequest.Email && !u.DeletedDate.HasValue);
                if (existingUserWithThisEmail != null)
                {
                    return new FailResponse<UserInfoResponse>
                    {
                        Message = "The email address already exists."
                    };
                }
            }

            if (updateUserRequest.Username != null && updateUserRequest.Username != currentUser?.Username)
            {
                User? existingUserWithThisUsername = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Username == updateUserRequest.Username && !u.DeletedDate.HasValue);
                if (existingUserWithThisUsername != null)
                {
                    return new FailResponse<UserInfoResponse>
                    {
                        Message = "The username already exists."
                    };
                }
            }

            currentUser = _mapper.Map<UpdateUserRequest, User>(updateUserRequest, currentUser!);
            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                return new SuccessResponse<UserInfoResponse>
                {
                    Message = "Your profile has been updated successfully.",
                    Data = _mapper.Map<UserInfoResponse>(currentUser)
                };
            }

            return new FailResponse<UserInfoResponse>
            {
                Message = "Failed to update your profile."
            };
        }

        public async Task<BaseResponse<ChangePasswordResponse>> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest)
        {
            User? currentUser = await _unitOfWork.Users.GetByIdAsync(GetCurrentUserId());

            string currentPasswordHash = PasswordHelper.HashUsingPbkdf2(changePasswordRequest.CurrentPassword!, Convert.FromBase64String(currentUser?.PasswordSalt!));
            if (currentUser?.PasswordHash != currentPasswordHash)
            {
                return new FailResponse<ChangePasswordResponse>
                {
                    Message = "Incorrect current password."
                };
            }

            if (changePasswordRequest.NewPassword != changePasswordRequest.ConfirmNewPassword)
            {
                return new FailResponse<ChangePasswordResponse>
                {
                    Message = "The new password and confirmation password do not match."
                };
            }

            byte[] newSalt = PasswordHelper.GetSecureSalt();
            string newPasswordHash = PasswordHelper.HashUsingPbkdf2(changePasswordRequest.NewPassword, newSalt);
            currentUser.PasswordHash = newPasswordHash;
            currentUser.PasswordSalt = Convert.ToBase64String(newSalt);
            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                var logoutResponse = await LogoutAsync();
                string message = "Your password has been changed.";
                if (logoutResponse.Status == StatusTypes.Success)
                {
                    message = "Your password has been changed. Please log in again with your new password.";
                }
                return new SuccessResponse<ChangePasswordResponse> { Message = message };
            }

            return new FailResponse<ChangePasswordResponse> { Message = "Failed to change the password." };
        }
    }
}
