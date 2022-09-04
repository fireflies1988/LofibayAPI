using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests.Collections;
using Domain.Models.DTOs.Responses.Collections;
using Domain.Models.DTOs.Responses.Photos;
using Domain.Models.ResponseTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LofibayAPI.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        private readonly ICollectionService _collectionService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CollectionController(ICollectionService collectionService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _collectionService = collectionService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateNewCollection(CreateCollectionRequest createCollectionRequest)
        {
            var createCollectionResponse = await _collectionService.CreateNewCollectionAsync(createCollectionRequest);
            if (createCollectionResponse.Status == StatusTypes.Fail)
            {
                return UnprocessableEntity(createCollectionResponse);
            }

            return Ok(createCollectionResponse);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> EditCollection(int id, EditCollectionRequest editCollectionRequest)
        {
            var editCollectionResponse = await _collectionService.UpdateCollectionAsync(id, editCollectionRequest);
            switch (editCollectionResponse.Status)
            {
                case StatusTypes.NotFound:
                    return NotFound(editCollectionResponse);
                case StatusTypes.Unauthorized:
                    return Unauthorized(editCollectionResponse);
                case StatusTypes.Success:
                    return Ok(editCollectionResponse);
                default:
                    return UnprocessableEntity(editCollectionResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ViewCollection(int id)
        {
            var response = await _collectionService.ViewCollectionByIdAsync(id);
            if (response.Status == StatusTypes.NotFound)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpGet("{id}/photos")]
        public async Task<IActionResult> ViewPhotosOfCollection(int id)
        {
            var response = await _collectionService.ViewPhotosOfCollectionAsync(id);
            if (response.Status == StatusTypes.NotFound)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("{id}/add-or-remove-photo")]
        public async Task<IActionResult> AddOrRemovePhotoToOrFromCollection(int id, [Required]int photoId)
        {
            var response = await _collectionService.AddOrRemovePhotoToOrFromCollectionAsync(photoId, id);
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCollection(int id)
        {
            var response = await _collectionService.DeleteCollectionAsync(id);
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

        [HttpGet]
        public async Task<IActionResult> GetAllCollections(string? keywords = null)
        {
            var collections = (await _unitOfWork.Collections.GetAllCollectionsAsync()).ToList();
            var viewUserCollectionsResponses = _mapper.Map<IEnumerable<Collection>, IEnumerable<ViewUserCollectionsResponse>>(collections).ToList();
            for (int i = 0; i < collections.Count(); i++)
            {
                viewUserCollectionsResponses[i].Thumbnails = _mapper.Map<IList<BasicPhotoInfoResponse?>>(
                    collections[i].PhotoCollections?.Select(pc => pc.Photo).Where(p => !p.DeletedDate.HasValue).Take(3).ToList());
                viewUserCollectionsResponses[i].NumOfPhotos = collections[i].PhotoCollections?.Select(pc => pc.Photo).Where(p => !p.DeletedDate.HasValue).Count() ?? 0;
            }

            if (keywords != null)
            {
                IList<ViewUserCollectionsResponse> filteredCollections = new List<ViewUserCollectionsResponse>();
                string[] keywordArr = keywords.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var c in viewUserCollectionsResponses)
                {
                    foreach(string keyword in keywordArr)
                    {
                        if (c.CollectionName?.ToLower().Contains(keyword) == true)
                        {
                            filteredCollections.Add(c);
                            break;
                        }

                        if (c.Description?.ToLower().Contains(keyword) == true)
                        {
                            filteredCollections.Add(c);
                            break;
                        }
                    }
                }

                return Ok(new SuccessResponse { Data = filteredCollections });
            }
            else
            {
                return Ok(new SuccessResponse { Data = viewUserCollectionsResponses });
            }   
        }
    }
}
