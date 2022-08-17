using Domain.Models.DTOs.Requests;
using Domain.Models.DTOs.Responses;
using Domain.Models.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<BaseResponse<TokenResponse>> LoginAsync(LoginRequest loginRequest);
        Task<BaseResponse<SignupResponse>> SignupAsync(SignupRequest signupRequest);
        Task<BaseResponse<LogoutResponse>> LogoutAsync(int userId);
    }
}
