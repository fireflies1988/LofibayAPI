using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests;
using Domain.Models.DTOs.Responses;
using Domain.Models.ResponseTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LofibayAPI.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public UserController(IUnitOfWork unitOfWork, ITokenService tokenService, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new FailResponse<TokenResponse>
                {
                    Message = "Missing login details."
                });
            }

            var loginResponse = await _userService.LoginAsync(loginRequest);
            if (loginResponse.Status == StatusTypes.Fail)
            {
                return Unauthorized(loginResponse);
            }

            return Ok(loginResponse);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            if (refreshTokenRequest == null || string.IsNullOrEmpty(refreshTokenRequest.RefreshToken) || refreshTokenRequest.UserId == 0)
            {
                return BadRequest(new FailResponse<TokenResponse>
                {
                    Message = "Missing refresh token details."
                });
            }
            
            var validateRefreshTokenResponse = await _tokenService.ValidateRefreshTokenAsync(refreshTokenRequest);
            if (validateRefreshTokenResponse.Status == StatusTypes.Fail)
            {
                return UnprocessableEntity(validateRefreshTokenResponse);
            }

            var tokenResponse = await _tokenService.GenerateTokensAsync((await _unitOfWork.Users.GetByIdAsync(validateRefreshTokenResponse.Data!.UserId))!);
            return Ok(new
            {
                AccessToken = tokenResponse?.Item1,
                RefreshToken = tokenResponse?.Item2
            });
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Join(SignupRequest signupRequest)
        {
            var signupResposne = await _userService.SignupAsync(signupRequest);

            if (signupResposne.Status == StatusTypes.Fail)
            {
                return UnprocessableEntity(signupResposne);
            }
            
            return Ok(signupResposne);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var logoutResponse = await _userService.LogoutAsync(userId);

            if (logoutResponse.Status == StatusTypes.Fail)
            {
                return UnprocessableEntity(logoutResponse);
            }

            return Ok(logoutResponse);
        }
    }
}
