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
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<BaseResponse<TokenResponse>> LoginAsync(LoginRequest loginRequest);
        Task<BaseResponse<SignupResponse>> SignupAsync(SignupRequest signupRequest);
        Task<BaseResponse<LogoutResponse>> LogoutAsync(int userId);
    }
}
