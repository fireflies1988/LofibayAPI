using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests;
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
            return Ok(await _photoService.UploadPhotoAsync(uploadPhotoRequest));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ViewPhotoDetails(int id)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhoto(int id, UpdatePhotoRequest updatePhotoRequest)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            throw new NotImplementedException();
        }
    }
}
