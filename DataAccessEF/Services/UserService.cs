using Common.Helpers;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests;
using Domain.Models.DTOs.Responses;
using Domain.Models.ResponseTypes;
using System.Linq.Expressions;

namespace DataAccessEF.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public UserService(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<BaseResponse<TokenResponse>> LoginAsync(LoginRequest loginRequest)
        {
            User? existingUser = (await _unitOfWork.Users.GetAsync(filter: u => u.Email == loginRequest.Email && !u.DeletedDate.HasValue, includeProperties: "RefreshTokens")).SingleOrDefault();
            if (existingUser == null)
            {
                return new FailResponse<TokenResponse>
                {
                    Message = "Email not found."
                };
            }

            // verify password hash
            string passwordHash = PasswordHelper.HashUsingPbkdf2(loginRequest.Password!, Convert.FromBase64String(existingUser.PasswordSalt!));
            if (existingUser.PasswordHash != passwordHash)
            {
                return new FailResponse<TokenResponse>
                {
                    Message = "Invalid password."
                };
            }

            var token = await _tokenService.GenerateTokensAsync(existingUser);
            return new SuccessResponse<TokenResponse>
            {
                Data = new TokenResponse
                {
                    AccessToken = token?.Item1,
                    RefreshToken = token?.Item2
                }
            };
        }

        public async Task<BaseResponse<LogoutResponse>> LogoutAsync(int userId)
        {
            RefreshToken? refreshToken = (await _unitOfWork.RefreshTokens.GetAsync(filter: rt => rt.UserId == userId)).FirstOrDefault();

            if (refreshToken == null)
            {
                return new SuccessResponse<LogoutResponse>();
            }

            _unitOfWork.RefreshTokens?.Remove(refreshToken);
            int saveResponse = await _unitOfWork.SaveChangesAsync();
            if (saveResponse >= 0)
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
            User? existingUser = (await _unitOfWork.Users!.GetAsync(u => u.Email == signupRequest.Email && !u.DeletedDate.HasValue)).FirstOrDefault();
            if (existingUser != null)
            {
                return new FailResponse<SignupResponse>
                {
                    Message = "An account already exists with the same email."
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

            var salt = PasswordHelper.GetSecureSalt();
            var passwordHash = PasswordHelper.HashUsingPbkdf2(signupRequest.Password!, salt);

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

            var saveResponse = await _unitOfWork.SaveChangesAsync();
            if (saveResponse >= 0)
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
    }
}
