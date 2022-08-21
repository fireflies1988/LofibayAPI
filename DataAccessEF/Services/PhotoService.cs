using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Common.Helpers;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests;
using Domain.Models.DTOs.Responses;
using Domain.Models.ResponseTypes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessEF.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IUserService _userService;

        public PhotoService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<BaseResponse<object>> UploadPhotoAsync(UploadPhotoRequest uploadPhotoRequest)
        {
            Cloudinary cloudinary = new Cloudinary(ConfigurationHelper.Configuration!["CloudinaryUrl"]);
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(_userService.GetCurrentUserId().ToString(), uploadPhotoRequest.ImageFile!.OpenReadStream()),
                Folder = $"Lofibay Storage Test/{_userService.GetCurrentUserId()}/Photos",
                Faces = true,
                Colors = true,
                Phash = true,
                ImageMetadata = true
            };
            var test = await cloudinary.UploadAsync(uploadParams);

            return new BaseResponse<object>
            {
                Data = test,
            };
        }
    }
}
