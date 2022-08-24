using Domain.Models.DTOs.Requests.Users;
using Domain.Models.DTOs.Responses.Users;
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
        int GetCurrentUserId();
        string GetCurrentEmail();
        string GetCurrentUsername();
        Task<BaseResponse<TokenResponse>> LoginAsync(LoginRequest loginRequest);
        Task<BaseResponse<SignupResponse>> SignupAsync(SignupRequest signupRequest);
        Task<BaseResponse<LogoutResponse>> LogoutAsync();
        Task<BaseResponse<UserInfoResponse>> UpdateUserAsync(UpdateUserRequest updateUserRequest);
        Task<BaseResponse<ChangePasswordResponse>> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest);
    }
}
