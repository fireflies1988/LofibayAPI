using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests.Photos;
using Domain.Models.DTOs.Responses.Photos;
using Domain.Models.ResponseTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LofibayAPI.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PhotoController(IPhotoService photoService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _photoService = photoService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadPhoto([FromForm]UploadPhotoRequest uploadPhotoRequest)
        {
            if (!uploadPhotoRequest.ImageFile!.ContentType.Contains("image"))
            {
                return BadRequest(new FailResponse { Message = "The input file accepts image formats only." });
            }

            try
            {
                var uploadPhotoResponse = await _photoService.UploadPhotoAsync(uploadPhotoRequest);
                if (uploadPhotoResponse.Status == StatusTypes.Fail)
                {
                    return UnprocessableEntity(uploadPhotoResponse);
                }
                return Ok(uploadPhotoResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPhotos()
        {
            return Ok(new SuccessResponse
            {
                Data = _mapper.Map<IEnumerable<Photo>, IEnumerable<BasicPhotoInfoResponse>>(await _unitOfWork.Photos.GetAsync(p => !p.DeletedDate.HasValue, includeProperties: "User,LikedPhotos,PhotoCollections"))
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ViewPhotoDetails(int id)
        {
            try
            {
                var response = await _photoService.ViewPhotoDetailsByIdAsync(id);
                if (response.Status == StatusTypes.NotFound)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhoto(int id, UpdatePhotoRequest updatePhotoRequest)
        {
            try
            {
                var updatePhotoResponse = await _photoService.UpdatePhotoAsync(id, updatePhotoRequest);
                if (updatePhotoResponse.Status == StatusTypes.NotFound)
                {
                    return NotFound(updatePhotoResponse);
                }
                if (updatePhotoResponse.Status == StatusTypes.Unauthorized)
                {
                    return Unauthorized(updatePhotoResponse);
                }

                return Ok(updatePhotoResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = ex.Message });
            }
        }

        [HttpPatch("{id}/download")]
        public async Task<IActionResult> IncreaseDownloadsByOne(int id)
        {
            Photo? existingPhoto = await _unitOfWork.Photos.GetFirstOrDefaultAsync(p => p.PhotoId == id && !p.DeletedDate.HasValue);
            if (existingPhoto == null)
            {
                return NotFound(new NotFoundResponse { Message = "Photo not found." });
            }

            existingPhoto.Downloads++;
            if ((await _unitOfWork.SaveChangesAsync()) > 0)
            {
                return Ok(new SuccessResponse { Message = "Downloads + 1" });
            }

            return UnprocessableEntity(new FailResponse { Message = "Something went wrong." });
        }

        [Authorize]
        [HttpPost("{id}/like-or-unlike")]
        public async Task<IActionResult> LikeOrUnlikePhoto(int id)
        {
            var response = await _photoService.LikeOrUnlikePhotoAsync(id);
            switch (response.Status)
            {
                case StatusTypes.NotFound:
                    return NotFound(response);
                case StatusTypes.Unauthorized:
                    return Unauthorized(response);
                case StatusTypes.Success:
                    return Ok(response);
                default:
                    return UnprocessableEntity(response);
            }
        }

        [Authorize]
        [HttpDelete("{id}/soft")]
        public async Task<IActionResult> SoftDeletePhoto(int id)
        {
            var response = await _photoService.SoftDeletePhotoAsync(id);
            switch (response.Status)
            {
                case StatusTypes.NotFound:
                    return NotFound(response);
                case StatusTypes.Unauthorized:
                    return Unauthorized(response);
                case StatusTypes.Success:
                    return Ok(response);
                default:
                    return UnprocessableEntity(response);
            }
        }

        [Authorize]
        [HttpDelete("{id}/hard")]
        public async Task<IActionResult> HardDeletePhoto(int id)
        {
            throw new NotImplementedException();
        }
    }
}
