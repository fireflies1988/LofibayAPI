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
        Task<BaseResponse<PhotoDetailsResponse>> ViewPhotoDetailsByIdAsync(int id);
        Task<BaseResponse<UpdatePhotoResponse>> UpdatePhotoAsync(int id, UpdatePhotoRequest updatePhotoRequest);
        Task InsertTagsAsync(Photo photo, IList<string> tags);
        Task<BaseResponse<IEnumerable<ViewYourUploadedPhotosResponse>>> ViewYourUploadedPhotosAysnc();
        Task<BaseResponse<IEnumerable<ViewYourLikedPhotosResponse>>> ViewYourLikedPhotoAsync();
        Task<BaseResponse<IEnumerable<BasicPhotoInfoResponse>>> ViewUserUploadedPhotosAsync(int id);
        Task<BaseResponse<IEnumerable<BasicPhotoInfoResponse>>> ViewLikedPhotosOfUserAsync(int id);
        Task<BaseResponse<object>> LikeOrUnlikePhotoAsync(int id);
        Task<BaseResponse<object>> SoftDeletePhotoAsync(int id);
    }
}
