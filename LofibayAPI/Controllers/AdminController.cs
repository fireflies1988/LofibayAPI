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
using WeCantSpell.Hunspell;

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

        [HttpGet("tags")]
        public async Task<IActionResult> GetAllTags(string? keyword = null, bool bad = false)
        {
            var tags = await _unitOfWork.Tags.GetAllAsync();
            if (bad == true)
            {
                IList<Tag> badTags = new List<Tag>();
                var dictionary = WordList.CreateFromFiles(@"en-US.dic");
                foreach (var tag in tags)
                {
                    string[] words = tag.Name!.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in words)
                    {
                        if (!dictionary.Check(word))
                        {
                            badTags.Add(tag);
                        }
                    }
                }

                
                if (keyword != null)
                {
                    IList<Tag> badTagsContainingKeyword = new List<Tag>();
                    foreach (var tag in badTags)
                    {
                        if (tag.Name?.ToLower().Contains(keyword.ToLower()) == true)
                        {
                            badTagsContainingKeyword.Add(tag);
                        }
                    }

                    return Ok(new SuccessResponse
                    {
                        Data = badTagsContainingKeyword
                    });
                }
                else
                {
                    return Ok(new SuccessResponse
                    {
                        Data = badTags
                    });
                }
            }

            if (keyword != null)
            {
                IList<Tag> tagsContainingKeyword = new List<Tag>();
                foreach (var tag in tags)
                {
                    if (tag.Name?.ToLower().Contains(keyword.ToLower()) == true)
                    {
                        tagsContainingKeyword.Add(tag);
                    }
                }

                return Ok(new SuccessResponse
                {
                    Data = tagsContainingKeyword
                });
            }
            else
            {
                return Ok(new SuccessResponse
                {
                    Data = tags
                });
            }
        }

        [HttpDelete("tags/{name}")]
        public async Task<IActionResult> RemoveTag(string name)
        {
            var existingTag = await _unitOfWork.Tags.GetByIdAsync(name);
            if (existingTag == null)
            {
                return NotFound(new NotFoundResponse { Message = "Tag not found." });
            }
            _unitOfWork.Tags.Remove(existingTag);
            if ((await _unitOfWork.SaveChangesAsync()) > 0)
            {
                return Ok(new SuccessResponse { Message = $"You have deleted tag '{name}' successfully." });
            }

            return UnprocessableEntity(new FailResponse { Message = "Unable to delete this tag." });
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var photos = await _unitOfWork.Photos.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();
            var collections = await _unitOfWork.Collections.GetAllAsync();

            return Ok(new SuccessResponse
            {
                Data = new StatsResponse
                {
                    NumberOfPhotos = photos.Count(),
                    NumberOfFeaturedPhotos = photos.Where(p => p.PhotoStateId == 2 && !p.DeletedDate.HasValue).Count(),
                    NumberOfUnfeaturedPhotos = photos.Where(p => p.PhotoStateId == 1 && !p.DeletedDate.HasValue).Count(),
                    NumberOfRejectedPhotos = photos.Where(p => p.PhotoStateId == 3 && !p.DeletedDate.HasValue).Count(),
                    NumberOfDeletedPhotos = photos.Where(p => p.DeletedDate.HasValue).Count(),

                    NumberOfCollections = collections.Count(),
                    NumberOfPrivateCollections = collections.Where(c => c.IsPrivate == true).Count(),
                    NumberOfPublicCollections = collections.Where(c => c.IsPrivate == false).Count(),

                    NumberOfUsers = users.Count(),
                    NumberOfUnverifiedUsers = users.Where(u => u.Verified == false).Count(),
                    NumberOfVerifiedUsers = users.Where(u => u.Verified == true).Count()
                }
            });
        }
    }
}
