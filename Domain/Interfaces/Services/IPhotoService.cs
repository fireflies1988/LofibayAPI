using Domain.Models.DTOs.Requests;
using Domain.Models.ResponseTypes;
using Microsoft.AspNetCore.Http;

namespace Domain.Interfaces.Services
{
    public interface IPhotoService
    {
        Task<BaseResponse<object>> UploadPhotoAsync(UploadPhotoRequest uploadPhotoRequest);
    }
}
