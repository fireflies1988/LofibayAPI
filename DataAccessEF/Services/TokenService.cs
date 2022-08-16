using Common.Helpers;
using Common.Models.Dto.Requests;
using Common.Models.Dto.Responses;
using Common.Models.ResponseTypes;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessEF.Services
{
    public class TokenService : ITokenService
    {
        private readonly LofibayDbContext _context;

        public TokenService(LofibayDbContext context)
        {
            _context = context;
        }

        public async Task<Tuple<string, string>?> GenerateTokensAsync(int userId)
        {
            var accessToken = TokenHelper.GenerateAccessToken(userId);
            var refreshToken = TokenHelper.GenerateRefreshToken();

            var user = await _context.Users!.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return null;
            }

            var salt = PasswordHelper.GetSecureSalt();
            var refreshTokenHashed = PasswordHelper.HashUsingPbkdf2(refreshToken, salt);

            if (user.RefreshTokens != null && user.RefreshTokens.Any())
            {
                await RemoveRefreshTokenAsync(user);
            }

            user.RefreshTokens?.Add(new RefreshToken
            {
                ExpirationDate = DateTime.Now.AddDays(30),
                UserId = userId,
                TokenHash = refreshTokenHashed,
                TokenSalt = Convert.ToBase64String(salt)
            });

            await _context.SaveChangesAsync();

            return new Tuple<string, string>(accessToken, refreshToken);
        }

        public async Task<bool> RemoveRefreshTokenAsync(User user)
        {
            var userRecord = await _context.Users!.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.UserId == user.UserId);
            if (userRecord == null)
            {
                return false;
            }

            if (userRecord.RefreshTokens != null && userRecord.RefreshTokens.Any())
            {
                var currentRefreshToken = userRecord.RefreshTokens.First();
                _context.RefreshTokens?.Remove(currentRefreshToken);
            }

            return true;
        }

        public async Task<BaseResponse<ValidateRefreshTokenResponse>> ValidateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            var refreshToken = await _context.RefreshTokens!.FirstOrDefaultAsync(rt => rt.UserId == refreshTokenRequest.UserId);
            if (refreshToken == null) {
                return new FailResponse<ValidateRefreshTokenResponse>
                {
                    Message = "Invalid session or user is already logged out."
                };
            }

            var refreshTokenValidateHash = PasswordHelper.HashUsingPbkdf2(refreshTokenRequest.RefreshToken!, Convert.FromBase64String(refreshToken.TokenSalt!));
            if (refreshToken.TokenHash != refreshTokenValidateHash)
            {
                return new FailResponse<ValidateRefreshTokenResponse>
                {
                    Message = "Invalid refresh token."
                };
            }

            if (refreshToken.ExpirationDate < DateTime.Now)
            {
                return new FailResponse<ValidateRefreshTokenResponse>
                {
                    Message = "Refresh token has expired."
                };
            }

            return new SuccessResponse<ValidateRefreshTokenResponse>
            {
                Data = new ValidateRefreshTokenResponse
                {
                    UserId = refreshToken.UserId
                }
            };
        }
    }
}
