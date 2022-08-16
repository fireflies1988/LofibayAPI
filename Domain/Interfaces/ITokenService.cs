using Common.Models.Dto.Requests;
using Common.Models.Dto.Responses;
using Common.Models.ResponseTypes;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITokenService
    {
        Task<Tuple<string, string>?> GenerateTokensAsync(int userId);
        Task<BaseResponse<ValidateRefreshTokenResponse>> ValidateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
        Task<bool> RemoveRefreshTokenAsync(User user);
    }
}
