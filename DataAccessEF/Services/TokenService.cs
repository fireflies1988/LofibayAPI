using Common.Helpers;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests;
using Domain.Models.DTOs.Responses;
using Domain.Models.ResponseTypes;
using Microsoft.Extensions.Configuration;

namespace DataAccessEF.Services
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Tuple<string, string>?> GenerateTokensAsync(User user)
        {
            if (user == null)
            {
                return null;
            }

            string accessToken = TokenHelper.GenerateAccessToken(user);
            string refreshToken = TokenHelper.GenerateRefreshToken();

            byte[] salt = PasswordHelper.GetSecureSalt();
            string refreshTokenHashed = PasswordHelper.HashUsingPbkdf2(refreshToken, salt);

            if (user.RefreshTokens != null && user.RefreshTokens.Any())
            {
                RemoveRefreshToken(user);
            }

            user.RefreshTokens?.Add(new RefreshToken
            {
                ExpirationDate = DateTime.Now.AddDays(ConfigurationHelper.Configuration.GetValue<double>("Jwt:RefreshTokenExpirationDays")),
                UserId = user.UserId,
                TokenHash = refreshTokenHashed,
                TokenSalt = Convert.ToBase64String(salt)
            });

            await _unitOfWork.SaveChangesAsync();

            return new Tuple<string, string>(accessToken, refreshToken);
        }

        public bool RemoveRefreshToken(User user)
        {
            if (user == null)
            {
                return false;
            }

            if (user.RefreshTokens != null && user.RefreshTokens.Any())
            {
                var currentRefreshToken = user.RefreshTokens.First();
                _unitOfWork.RefreshTokens?.Remove(currentRefreshToken);
            }

            return true;
        }

        public async Task<BaseResponse<ValidateRefreshTokenResponse>> ValidateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            RefreshToken? refreshToken = (await _unitOfWork.RefreshTokens!.GetAsync(filter: rt => rt.UserId == refreshTokenRequest.UserId)).FirstOrDefault();
            if (refreshToken == null)
            {
                return new FailResponse<ValidateRefreshTokenResponse>
                {
                    Message = "Invalid session or user is already logged out."
                };
            }

            string refreshTokenValidateHash = PasswordHelper.HashUsingPbkdf2(refreshTokenRequest.RefreshToken!, Convert.FromBase64String(refreshToken.TokenSalt!));
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
