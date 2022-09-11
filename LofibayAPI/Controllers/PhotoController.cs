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
        public async Task<IActionResult> UploadPhoto([FromForm] UploadPhotoRequest uploadPhotoRequest)
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
        public async Task<IActionResult> GetAllPhotos(string? keywords = null, string orientation = "any", string color = "any", string sortBy = "relevance")
        {
            if (keywords != null)
            {
                IList<Photo> photos = new List<Photo>();
                if (orientation == "portrait")
                {
                    photos = (await _unitOfWork.Photos.GetAsync(
                        filter: p => !p.DeletedDate.HasValue && p.PhotoStateId == PhotoStates.Featured && !p.User!.DeletedDate.HasValue
                            && p.Width < p.Height,
                        includeProperties: "User,LikedPhotos,PhotoCollections,PhotoTags,PhotoColors")).ToList();
                }
                else if (orientation == "landscape")
                {
                    photos = (await _unitOfWork.Photos.GetAsync(
                        filter: p => !p.DeletedDate.HasValue && p.PhotoStateId == PhotoStates.Featured && !p.User!.DeletedDate.HasValue
                            && p.Width > p.Height,
                        includeProperties: "User,LikedPhotos,PhotoCollections,PhotoTags,PhotoColors")).ToList();
                }
                else if (orientation == "square")
                {
                    photos = (await _unitOfWork.Photos.GetAsync(
                        filter: p => !p.DeletedDate.HasValue && p.PhotoStateId == PhotoStates.Featured && !p.User!.DeletedDate.HasValue
                            && p.Width == p.Height,
                        includeProperties: "User,LikedPhotos,PhotoCollections,PhotoTags,PhotoColors")).ToList();
                }
                else // any orientation
                {
                    photos = (await _unitOfWork.Photos.GetAsync(
                        filter: p => !p.DeletedDate.HasValue && p.PhotoStateId == PhotoStates.Featured && !p.User!.DeletedDate.HasValue,
                        includeProperties: "User,LikedPhotos,PhotoCollections,PhotoTags,PhotoColors")).ToList();
                }

                if (color != "any")
                {
                    IList<Photo> photosAfterFilteringColor = new List<Photo>();
                    foreach (var photo in photos)
                    {
                        foreach (var photoColor in photo.PhotoColors!.OrderByDescending(p => p.PredominantPercent))
                        {
                            if (photoColor.ColorName == color)
                            {
                                photosAfterFilteringColor.Add(photo);
                                break;
                            }
                        }
                    }
                    photos = photosAfterFilteringColor;
                }

                if (sortBy == "newest")
                {
                    photos = photos.OrderByDescending(p => p.UploadedAt).ToList();
                }

                IList<BasicPhotoInfoResponse> filteredPhotos = new List<BasicPhotoInfoResponse>();
                string[] keywordArr = keywords.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var photo in photos)
                {
                    foreach (string keyword in keywordArr)
                    {
                        bool matchedWithTag = false;

                        if (photo.Description?.ToLower().Contains(keyword.ToLower()) == true)
                        {
                            filteredPhotos.Add(_mapper.Map<Photo, BasicPhotoInfoResponse>(photo));
                            break;
                        }

                        foreach (var tag in photo.PhotoTags!)
                        {
                            if (tag.TagName?.ToLower().Contains(keyword.ToLower()) == true)
                            {
                                matchedWithTag = true;
                                filteredPhotos.Add(_mapper.Map<Photo, BasicPhotoInfoResponse>(photo));
                                break;
                            }
                        }

                        if (matchedWithTag == true)
                        {
                            break;
                        }
                    }
                }

                return Ok(new SuccessResponse
                {
                    Data = filteredPhotos
                });
            }
            else
            {
                Random random = new Random();
                return Ok(new SuccessResponse
                {
                    Data = _mapper.Map<IEnumerable<Photo>, IEnumerable<BasicPhotoInfoResponse>>(
                        (await _unitOfWork.Photos.GetAsync(
                            filter: p => !p.DeletedDate.HasValue && p.PhotoStateId == PhotoStates.Featured && !p.User!.DeletedDate.HasValue,
                            includeProperties: "User,LikedPhotos,PhotoCollections")).OrderBy(p => random.Next()))
                });
            }
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
