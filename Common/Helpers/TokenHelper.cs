using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Common.Helpers
{
    public class TokenHelper
    {
        public static string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Convert.FromBase64String(ConfigurationHelper.Configuration!["Jwt:SecretKey"]);

            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.Username!)
            });

            var signingCredentials = new SigningCredentials(key: new SymmetricSecurityKey(secretKey), algorithm: SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Issuer = ConfigurationHelper.Configuration["Jwt:ValidIssuer"],
                Audience = ConfigurationHelper.Configuration["Jwt:ValidAudience"],
                Expires = DateTime.Now.AddMinutes(ConfigurationHelper.Configuration.GetValue<double>("Jwt:AccessTokenExpirationMinutes")),
                SigningCredentials = signingCredentials,
            };

            return tokenHandler.CreateEncodedJwt(tokenDescriptor);
        }

        public static string GenerateRefreshToken()
        {
            var secureRandomBytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(secureRandomBytes);
        }
    }
}
