using Domain.Models.DTOs.Requests;
using Domain.Models.DTOs.Responses;
using Domain.Models.ResponseTypes;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface ITokenService
    {
        Task<Tuple<string, string>?> GenerateTokensAsync(User? user);
        Task<BaseResponse<ValidateRefreshTokenResponse>> ValidateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
        bool RemoveRefreshToken(User user);
    }
}
