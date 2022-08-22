using Domain.Enums;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests;
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

        public PhotoController(IPhotoService photoService)
        {
            _photoService = photoService;
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
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorReponse { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ViewPhotoDetails(int id)
        {
            try
            {
                var response = await _photoService.GetPhotoDetailsByIdAsync(id);
                if (response.Status == StatusTypes.NotFound)
                {
                    return NotFound(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorReponse { Message = ex.Message });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorReponse { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            throw new NotImplementedException();
        }
    }
}
