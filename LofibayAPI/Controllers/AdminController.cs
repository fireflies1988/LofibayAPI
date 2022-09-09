using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.DTOs.Responses.Admin;
using Domain.Models.DTOs.Responses.Users;
using Domain.Models.ResponseTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LofibayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("Users")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(new SuccessResponse<IEnumerable<UserInfoResponse>>
            {
                Data = _mapper.Map<IEnumerable<UserInfoResponse>>(await _unitOfWork.Users.GetAllAsync())
            });
        }

        [HttpGet("photos")]
        public async Task<IActionResult> GetPhotos(int state = PhotoStates.NotReviewed, string orderBy = "uploadedat", bool desc = false)
        {
            IEnumerable<Photo> photos = new List<Photo>();
            switch (orderBy.ToLower())
            {
                case "uploadedat":
                    photos = await _unitOfWork.Photos.GetAsync(
                        filter: p => p.PhotoStateId == state && !p.DeletedDate.HasValue, 
                        orderBy: p => desc == true ? p.OrderByDescending(p => p.UploadedAt) : p.OrderBy(p => p.UploadedAt));
                    break;
                case "facesdetected":
                    photos = await _unitOfWork.Photos.GetAsync(
                        filter: p => p.PhotoStateId == state && !p.DeletedDate.HasValue,
                        orderBy: p => desc == true ? p.OrderByDescending(p => p.FacesDetected) : p.OrderBy(p => p.FacesDetected));
                    break;
                default:
                    return BadRequest(new FailResponse { Message = "Invalid orderBy value." });
            }

            return Ok(new SuccessResponse<IEnumerable<PhotoInfoResponse>>
            {
                Data = _mapper.Map<IEnumerable<PhotoInfoResponse>>(photos)
            });
        }

        [HttpGet("photos/deleted")]
        public async Task<IActionResult> GetDeletedPhotos(string orderBy = "uploadedat", bool desc = false)
        {
            IEnumerable<Photo> photos = new List<Photo>();
            switch (orderBy.ToLower())
            {
                case "uploadedat":
                    photos = await _unitOfWork.Photos.GetAsync(
                        filter: p => p.DeletedDate.HasValue,
                        orderBy: p => desc == true ? p.OrderByDescending(p => p.UploadedAt) : p.OrderBy(p => p.UploadedAt));
                    break;
                case "facesdetected":
                    photos = await _unitOfWork.Photos.GetAsync(
                        filter: p => p.DeletedDate.HasValue,
                        orderBy: p => desc == true ? p.OrderByDescending(p => p.FacesDetected) : p.OrderBy(p => p.FacesDetected));
                    break;
                case "deleteddate":
                    photos = await _unitOfWork.Photos.GetAsync(
                        filter: p => p.DeletedDate.HasValue,
                        orderBy: p => desc == true ? p.OrderByDescending(p => p.DeletedDate) : p.OrderBy(p => p.DeletedDate));
                    break;
                default:
                    return BadRequest(new FailResponse { Message = "Invalid orderBy value." });
            }

            return Ok(new SuccessResponse<IEnumerable<PhotoInfoResponse>>
            {
                Data = _mapper.Map<IEnumerable<PhotoInfoResponse>>(photos)
            });
        }

        [HttpPatch("photos/{id}")]
        public async Task<IActionResult> UpdatePhotoState(int id, [Required]int photoStateId)
        {
            Photo? existingPhoto = await _unitOfWork.Photos.GetFirstOrDefaultAsync(p => p.PhotoId == id && !p.DeletedDate.HasValue);
            if (existingPhoto == null)
            {
                return NotFound(new NotFoundResponse { Message = "Photo not found." });
            }

            string message;
            switch (photoStateId)
            {
                case 1:
                    message = "This photo has been set state 'NotReviewed'.";
                    break;
                case 2:
                    message = "This photo has been featured.";
                    await _unitOfWork.Notifications.AddAsync(new Notification
                    {
                        ImageUrl = existingPhoto.PhotoUrl,
                        Message =  "Congratulations! Your photo has been featured.",
                        UserId = existingPhoto.UserId,
                    });
                    break;
                case 3:
                    message = "This photo has been rejected.";
                    break;
                default:
                    return BadRequest(new FailResponse { Message = "Invalid photoStateId value." });
            }
            existingPhoto.PhotoStateId = photoStateId;
            if ((await _unitOfWork.SaveChangesAsync()) > 0)
            {
                return Ok(new SuccessResponse { Message = message });
            }

            return UnprocessableEntity(new FailResponse { Message = "Something went wrong, unable to update state of this photo." });
        }
    }
}
