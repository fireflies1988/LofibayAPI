using Domain.Models.DTOs.Requests;
using Domain.Models.DTOs.Responses;
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
        Task<BaseResponse<object>> AddPhotoToCollectionAsync(int photoId, int collectionId);
        Task<BaseResponse<object>> RemovePhotoFromCollectionAsync(int photoId, int collectionId);
        Task<BaseResponse<IEnumerable<CurrentUserCollectionResponse>>> GetCurrentUserCollectionsAsync();
        Task<BaseResponse<IEnumerable<CurrentUserPhotoDetailsResponse>>> GetPhotosOfCurrentUserCollectionAsync(int collectionId);
        Task<BaseResponse<object>> DeleteCollectionAsync(int id);
    }
}
