﻿using AutoMapper;
using Common.Helpers;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests.Emails;
using Domain.Models.DTOs.Requests.Users;
using Domain.Models.DTOs.Responses.Users;
using Domain.Models.ResponseTypes;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Cryptography;

namespace DataAccessEF.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, ITokenService tokenService, IHttpContextAccessor httpContextAccessor, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _emailService = emailService;
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

            byte[] passwordSalt = PasswordHelper.GetSecureSalt();
            string passwordHash = PasswordHelper.HashUsingPbkdf2(signupRequest.Password!, passwordSalt);
            string verificationToken = TokenHelper.GenerateRefreshToken();
            byte[] verificationTokenSalt = PasswordHelper.GetSecureSalt();
            string verificationTokenHash = PasswordHelper.HashUsingPbkdf2(verificationToken, verificationTokenSalt);
            var user = new User
            {
                Email = signupRequest.Email,
                Username = signupRequest.Username,
                PasswordHash = passwordHash,
                PasswordSalt = Convert.ToBase64String(passwordSalt),
                FirstName = signupRequest.FirstName,
                LastName = signupRequest.LastName,
                VerificationTokenHash = verificationTokenHash,
                VerificationTokenSalt = Convert.ToBase64String(verificationTokenSalt),
                VerificationTokenExpDate = DateTime.Now.AddMinutes(5)
            };
            await _unitOfWork.Users.AddAsync(user);

            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                await _emailService.SendVerificationCode(new VerificationEmailRequest
                {
                    To = signupRequest.Email,
                    VerificationCode = verificationToken
                });
                return new SuccessResponse<SignupResponse>
                {
                    Message = "Your account has been successfully created. Please check your email for your account verification code and enter it below to confirm your email address.",
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

        public async Task<BaseResponse<object>> VerifyAsync(VerifyAccountRequest verifyAccountRequest)
        {
            User? currentUser = await _unitOfWork.Users.GetByIdAsync(GetCurrentUserId());
            if (currentUser!.Verified)
            {
                return new SuccessResponse { Message = "Your account has already been verified." };
            }

            if (currentUser.VerificationTokenExpDate < DateTime.Now)
            {
                return new FailResponse { Message = "The verification code has been expired. Please click 'Resend' to get a new verification code." };
            }

            string verificationCodeHash = PasswordHelper.HashUsingPbkdf2(verifyAccountRequest.VerificationCode!, Convert.FromBase64String(currentUser.VerificationTokenSalt!));
            if (currentUser.VerificationTokenHash != verificationCodeHash)
            {
                return new FailResponse { Message = "Invalid verification code." };
            }

            currentUser.Verified = true;
            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                return new SuccessResponse { Message = "Your account has been verified." };
            }

            return new FailResponse { Message = "Something went wrong, unable to verify your accoount." }; 
        }

        public async Task<BaseResponse<object>> ResendVerificationCode()
        {
            User currentUser = (await _unitOfWork.Users.GetByIdAsync(GetCurrentUserId()))!;
            byte[] newVerificationTokenSalt = PasswordHelper.GetSecureSalt();
            string newVerificationToken = TokenHelper.GenerateRefreshToken();
            string newVerificationTokenHash = PasswordHelper.HashUsingPbkdf2(newVerificationToken, newVerificationTokenSalt);
            currentUser.VerificationTokenHash = newVerificationTokenHash;
            currentUser.VerificationTokenSalt = Convert.ToBase64String(newVerificationTokenSalt);
            currentUser.VerificationTokenExpDate = DateTime.Now.AddMinutes(5);

            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                return await _emailService.SendVerificationCode(new VerificationEmailRequest
                {
                    To = currentUser.Email,
                    VerificationCode = newVerificationToken
                });
            }

            return new FailResponse { Message = "Something went wrong." };
        }

        public async Task<BaseResponse<object>> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest)
        {
            
            User? existingUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Email == forgotPasswordRequest.Email && !u.DeletedDate.HasValue);
            if (existingUser == null)
            {
                return new FailResponse { Message = "Invalid email." };
            }

            if (!existingUser.Verified)
            {
                return new FailResponse { Message = "This email is not verified." };
            }

            byte[] newResetTokenSalt = PasswordHelper.GetSecureSalt();
            string newPassword = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            string newResetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            string newResetTokenHash = PasswordHelper.HashUsingPbkdf2(newResetToken, newResetTokenSalt);
            existingUser.ResetTokenHash = newResetTokenHash;
            existingUser.ResetTokenSalt = Convert.ToBase64String(newResetTokenSalt);
            existingUser.ResetTokenExpDate = DateTime.Now.AddDays(5);

            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                return await _emailService.SendEmailAsync(new EmailRequest
                {
                    To = forgotPasswordRequest.Email,
                    Subject = "Lofibay - Forgot password",
                    Body = @$"<p>Your new password: <b>{newPassword}</b></p>
                        <p>Please click this <a href='https://localhost:7039/api/users/password/reset?email={forgotPasswordRequest.Email}&password={newPassword}&resetToken={newResetToken}'>link</a>
                        to confirm this reset password request.</p>
                        <p>Just ignore this email if you didn't request this."
                });
            }

            return new FailResponse { Message = "Something went wrong, unable to send forgot password email." };
        }

        public async Task<BaseResponse<object>> ResetPasswordAsync(string email, string password, string resetToken)
        {
            User existingUser = (await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Email == email && !u.DeletedDate.HasValue))!;
            string resetTokenHash = PasswordHelper.HashUsingPbkdf2(resetToken, Convert.FromBase64String(existingUser.ResetTokenSalt!));
            if (existingUser.ResetTokenHash != resetTokenHash)
            {
                return new FailResponse { Message = "Invalid reset token." };
            }

            if (existingUser.ResetTokenExpDate < DateTime.Now)
            {
                return new FailResponse { Message = "Reset token expired." };
            }
            
            byte[] newPasswordSalt = PasswordHelper.GetSecureSalt();
            string newPasswordHash = PasswordHelper.HashUsingPbkdf2(password, newPasswordSalt);
            existingUser.PasswordHash = newPasswordHash;
            existingUser.PasswordSalt = Convert.ToBase64String(newPasswordSalt);
            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                return new SuccessResponse { Message = "Your password has been reset. Please log in with your new password." };
            }

            return new FailResponse { Message = "Failed to reset your password." };
        }
    }
}
