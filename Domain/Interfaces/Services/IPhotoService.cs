using Domain.Entities;
using Domain.Models.DTOs.Requests.Photos;
using Domain.Models.DTOs.Responses.Photos;
using Domain.Models.ResponseTypes;
using Microsoft.AspNetCore.Http;

namespace Domain.Interfaces.Services
{
    public interface IPhotoService
    {
        Task<BaseResponse<object>> UploadPhotoAsync(UploadPhotoRequest uploadPhotoRequest);
        Task<BaseResponse<PhotoDetailsResponse>> GetPhotoDetailsByIdAsync(int id);
        Task<BaseResponse<UpdatePhotoResponse>> UpdatePhotoAsync(int id, UpdatePhotoRequest updatePhotoRequest);
        Task InsertTagsAsync(Photo photo, IList<string> tags);
        Task<BaseResponse<IEnumerable<ViewYourUploadedPhotosResponse>>> ViewYourUploadedPhotosAysnc();
        Task<BaseResponse<IEnumerable<ViewYourLikedPhotosResponse>>> ViewYourLikedPhotoAsync();
        Task<BaseResponse<object>> LikeOrUnlikePhoto(int id);
        Task<BaseResponse<object>> DeletePhotoAsync(int id);
    }
}
