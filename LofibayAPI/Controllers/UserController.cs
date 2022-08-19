﻿using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests;
using Domain.Models.DTOs.Responses;
using Domain.Models.ResponseTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LofibayAPI.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUnitOfWork unitOfWork, ITokenService tokenService, IUserService userService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
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

            var tokenResponse = await _tokenService.GenerateTokensAsync(await _unitOfWork.Users.GetFirstOrDefaultAsync(filter: u => u.UserId == validateRefreshTokenResponse.Data!.UserId, includeProperties: "Role,RefreshTokens"));
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
        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            var logoutResponse = await _userService.LogoutAsync();

            if (logoutResponse.Status == StatusTypes.Fail)
            {
                return UnprocessableEntity(logoutResponse);
            }

            return Ok(logoutResponse);
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            return Ok(new SuccessResponse<UserInfoResponse>
            {
                Data = _mapper.Map<UserInfoResponse>(await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.UserId == _userService.GetCurrentUserId(), includeProperties: "Address,Gender"))
            });
        }

        [Authorize]
        [HttpPatch("current")]
        public async Task<IActionResult> EditProfile(UpdateUserRequest updateUserRequest)
        {
            var updateUserResponse = await _userService.UpdateUserAsync(updateUserRequest);
            if (updateUserResponse.Status == StatusTypes.Fail)
            {
                return UnprocessableEntity(updateUserResponse);
            }

            return Ok(updateUserResponse);
        }

        [Authorize]
        [HttpPatch("current/password/change")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            var changePasswordResponse = await _userService.ChangePasswordAsync(changePasswordRequest);
            if (changePasswordResponse.Status == StatusTypes.Fail)
            {
                return UnprocessableEntity(changePasswordResponse);
            }

            return Ok(changePasswordResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserInfoById(int id)
        {
            return Ok(new SuccessResponse<UserInfoResponse>
            {
                Data = _mapper.Map<UserInfoResponse>(await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.UserId == id, includeProperties: "Address,Gender"))
            });
        }
    }
}