using Domain.Models.DTOs.Requests.Collections;
using Domain.Models.DTOs.Responses.Collections;
using Domain.Models.DTOs.Responses.Photos;
using Domain.Models.DTOs.Responses.Users;
using Domain.Models.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Services
{
    public interface ICollectionService
    {
        Task<BaseResponse<CreateCollectionResponse>> CreateNewCollectionAsync(CreateCollectionRequest createCollectionRequest);
        Task<BaseResponse<EditCollectionResponse>> UpdateCollectionAsync(int id, EditCollectionRequest editCollectionRequest);
        Task<BaseResponse<ViewCollectionResponse>> ViewCollectionByIdAsync(int id);
        Task<BaseResponse<IEnumerable<PhotoDetailsResponse>>> ViewPhotosOfCollectionAsync(int collectionId);
        Task<BaseResponse<object>> AddOrRemovePhotoToOrFromCollectionAsync(int photoId, int collectionId);
        Task<BaseResponse<IEnumerable<CurrentUserCollectionResponse>>> GetCurrentUserCollectionsAsync();
        Task<BaseResponse<IEnumerable<CurrentUserPhotoDetailsResponse>>> GetPhotosOfCurrentUserCollectionAsync(int collectionId);
        Task<BaseResponse<object>> DeleteCollectionAsync(int id);
        Task<BaseResponse<IEnumerable<ViewUserCollectionsResponse>>> ViewUserCollectionsAsync(int userId);
        Task<BaseResponse<IEnumerable<BasicPhotoInfoResponse>>> ViewPhotosOfUserCollectionAsync(int userId, int collectionId);
    }
}
