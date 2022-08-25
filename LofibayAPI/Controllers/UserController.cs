using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests.Users;
using Domain.Models.DTOs.Responses.Users;
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
        private readonly ICollectionService _collectionService;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;

        public UserController(IUnitOfWork unitOfWork, ITokenService tokenService, IUserService userService, IMapper mapper, ICollectionService collectionService, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _userService = userService;
            _mapper = mapper;
            _collectionService = collectionService;
            _photoService = photoService;
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

        [Authorize]
        [HttpPost("current/verify")]
        public async Task<IActionResult> Verify(VerifyAccountRequest verifyAccountRequest)
        {
            var response = await _userService.VerifyAsync(verifyAccountRequest);
            if (response.Status == StatusTypes.Fail)
            {
                return UnprocessableEntity(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("current/resend-verification-code")]
        public async Task<IActionResult> ResendVerificationCode()
        {
            var response = await _userService.ResendVerificationCode();
            if (response.Status == StatusTypes.Fail)
            {
                return UnprocessableEntity(response);
            }

            return Ok(response);
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

        [HttpPost("password/forgot")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            var response = await _userService.ForgotPasswordAsync(forgotPasswordRequest);
            if (response.Status == StatusTypes.Fail)
            {
                return UnprocessableEntity(response);
            }

            return Ok(response);
        }

        [HttpGet("Password/reset")]
        public async Task<IActionResult> ResetPassword(string email, string password, string resetToken)
        {
            var response = await _userService.ResetPasswordAsync(email, password, resetToken);
            if (response.Status == StatusTypes.Fail)
            {
                return UnprocessableEntity(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("current/collections")]
        public async Task<IActionResult> GetCurrentUserCollections()
        {
            return Ok(await _collectionService.GetCurrentUserCollectionsAsync());
        }

        [Authorize]
        [HttpGet("current/collections/{id}/photos")]
        public async Task<IActionResult> ViewPhotosOfYourCollection(int id)
        {
            var response = await _collectionService.GetPhotosOfCurrentUserCollectionAsync(id);
            if (response.Status == StatusTypes.NotFound)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet("current/photos")]
        public async Task<IActionResult> ViewYourUploadedPhotos()
        {
            var response = await _photoService.ViewYourUploadedPhotosAysnc();
            return Ok(response);
        }

        [Authorize]
        [HttpGet("current/liked-photos")]
        public async Task<IActionResult> ViewYourLikedPhotos()
        {
            var response = await _photoService.ViewYourLikedPhotoAsync();
            return Ok(response);
        }

        [HttpGet("{id}/collections")]
        public async Task<IActionResult> ViewCollectionsOfUser(int id)
        {
            var response = await _collectionService.ViewUserCollectionsAsync(id);
            if (response.Status == StatusTypes.NotFound)
            {
                return UnprocessableEntity(response);
            }
            return Ok(response);
        }

        [HttpGet("{userId}/collections/{collectionId}/photos")]
        public async Task<IActionResult> ViewPhotosOfUserCollection(int userId, int collectionId)
        {
            var response = await _collectionService.ViewPhotosOfUserCollectionAsync(userId, collectionId);
            if (response.Status == StatusTypes.NotFound)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("{id}/photos")]
        public async Task<IActionResult> ViewUserUploadedPhoto(int id)
        {
            var response = await _photoService.ViewUserUploadedPhotosAsync(id);
            if (response.Status == StatusTypes.NotFound)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet("{id}/liked-photos")]
        public async Task<IActionResult> ViewLikedPhotosOfUser(int id)
        {
            var response = await _photoService.ViewLikedPhotosOfUserAsync(id);
            if (response.Status == StatusTypes.NotFound)
            {
                return NotFound(response);
            }
            return Ok(response);
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
