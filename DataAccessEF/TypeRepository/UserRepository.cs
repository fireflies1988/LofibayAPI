using Common.Helpers;
using Common.Models.Dto.Requests;
using Common.Models.Dto.Responses;
using Common.Models.ResponseTypes;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessEF.TypeRepository
{
    class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ITokenService _tokenService;
        public UserRepository(LofibayDbContext context, ITokenService tokenService) : base(context)
        {
            _tokenService = tokenService;
        }

        public async Task<BaseResponse<TokenResponse>> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _context.Users!.SingleOrDefaultAsync(u => u.Email == loginRequest.Email && !u.DeletedDate.HasValue);
            if (user == null)
            {
                return new FailResponse<TokenResponse>
                {
                    Message = "Email not found."
                };
            }

            var passwordHash = PasswordHelper.HashUsingPbkdf2(loginRequest.Password!, Convert.FromBase64String(user.PasswordSalt!));
            if (user.PasswordHash != passwordHash)
            {
                return new FailResponse<TokenResponse>
                {
                    Message = "Invalid password."
                };
            }

            var token = await _tokenService.GenerateTokensAsync(user.UserId);
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
            var refreshToken = await _context.RefreshTokens!.FirstOrDefaultAsync(rt => rt.UserId == userId);

            if (refreshToken == null)
            {
                return new SuccessResponse<LogoutResponse>();
            }

            _context.RefreshTokens?.Remove(refreshToken);
            var saveResponse = await _context.SaveChangesAsync();
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
            var existingUser = await _context.Users!.SingleOrDefaultAsync(u => u.Email == signupRequest.Email);
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
            await _context.Users!.AddAsync(user);

            var saveResponse = await _context.SaveChangesAsync();
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
