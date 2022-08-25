using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests.Collections;
using Domain.Models.DTOs.Responses.Collections;
using Domain.Models.DTOs.Responses.Photos;
using Domain.Models.DTOs.Responses.Users;
using Domain.Models.ResponseTypes;

namespace DataAccessEF.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public CollectionService(IMapper mapper, IUserService userService, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<CreateCollectionResponse>> CreateNewCollectionAsync(CreateCollectionRequest createCollectionRequest)
        {
            Collection newCollection = _mapper.Map<CreateCollectionRequest, Collection>(createCollectionRequest);
            newCollection.UserId = _userService.GetCurrentUserId();
            await _unitOfWork.Collections.AddAsync(newCollection);
            if (await _unitOfWork.SaveChangesAsync() == 0)
            {
                return new FailResponse<CreateCollectionResponse> { Message = "Failed to create a new collection." };
            }

            if (createCollectionRequest.PhotoId.HasValue && createCollectionRequest.PhotoId != 0)
            {
                Photo? existingPhoto = await _unitOfWork.Photos.GetByIdAsync(createCollectionRequest.PhotoId);
                if (existingPhoto != null)
                {
                    newCollection.PhotoCollections = new List<PhotoCollection>
                    {
                        new PhotoCollection
                        {
                            CollectionId = newCollection.CollectionId,
                            PhotoId = existingPhoto.PhotoId
                        }
                    };

                    await _unitOfWork.SaveChangesAsync();
                }
            }

            return new SuccessResponse<CreateCollectionResponse>
            {
                Message = "A new collection has been created successfully.",
                Data = _mapper.Map<Collection, CreateCollectionResponse>(newCollection)
            };
        }

        public async Task<BaseResponse<IEnumerable<CurrentUserCollectionResponse>>> GetCurrentUserCollectionsAsync()
        {
            return new SuccessResponse<IEnumerable<CurrentUserCollectionResponse>>
            {
                Data = _mapper.Map<IEnumerable<Collection>, IEnumerable<CurrentUserCollectionResponse>>(
                    await _unitOfWork.Collections.GetAsync(c => c.UserId == _userService.GetCurrentUserId()))
            };
        }

        public async Task<BaseResponse<IEnumerable<CurrentUserPhotoDetailsResponse>>> GetPhotosOfCurrentUserCollectionAsync(int collectionId)
        {
            Collection? existingCollection = await _unitOfWork.Collections.GetCollectionIncludingPhotosByIdAsync(collectionId, _userService.GetCurrentUserId());
            if (existingCollection == null)
            {
                return new NotFoundResponse<IEnumerable<CurrentUserPhotoDetailsResponse>> { Message = "Collection not found." };
            }

            return new SuccessResponse<IEnumerable<CurrentUserPhotoDetailsResponse>>
            {
                Data = _mapper.Map<IEnumerable<Photo?>, IEnumerable<CurrentUserPhotoDetailsResponse>>(existingCollection.PhotoCollections!.Select(pc => pc.Photo).Where(p => !p.DeletedDate.HasValue))
            };
        }

        public async Task<BaseResponse<object>> AddOrRemovePhotoToOrFromCollectionAsync(int photoId, int collectionId)
        {
            Collection? existingCollection = await _unitOfWork.Collections.GetFirstOrDefaultAsync(c => c.CollectionId == collectionId, "PhotoCollections");
            if (existingCollection == null)
            {
                return new NotFoundResponse { Message = "Collection not found." };
            }

            if (existingCollection.UserId != _userService.GetCurrentUserId())
            {
                return new UnauthorizedResponse { Message = "You don't own this collection." };
            }

            Photo? existingPhoto = await _unitOfWork.Photos.GetFirstOrDefaultAsync(p => p.PhotoId == photoId && !p.DeletedDate.HasValue);
            if (existingPhoto == null)
            {
                return new NotFoundResponse { Message = "Photo not found." };
            }

            PhotoCollection? existingPhotoCollection = existingCollection.PhotoCollections!.FirstOrDefault(pc => pc.PhotoId == photoId && pc.CollectionId == collectionId);
            if (existingPhotoCollection != null)
            {
                // remove from collection
                existingCollection.PhotoCollections!.Remove(existingPhotoCollection);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return new SuccessResponse { Message = $"Removed this photo from your collection '{existingCollection.CollectionName}'" };
                }
            }
            else
            {
                // add to collection
                existingCollection.PhotoCollections!.Add(new PhotoCollection
                {
                    PhotoId = photoId,
                    CollectionId = collectionId
                });

                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return new SuccessResponse { Message = $"This photo has been added to your collection '{existingCollection.CollectionName}'" };
                }
            }

            return new FailResponse { Message = "Something went wrong, unable to add/remove this photo to/from your collection." };
        }

        public async Task<BaseResponse<EditCollectionResponse>> UpdateCollectionAsync(int id, EditCollectionRequest editCollectionRequest)
        {
            Collection? existingCollection = await _unitOfWork.Collections.GetByIdAsync(id);
            if (existingCollection == null)
            {
                return new NotFoundResponse<EditCollectionResponse> { Message = "Collection not found." };
            }

            if (existingCollection.UserId != _userService.GetCurrentUserId())
            {
                return new UnauthorizedResponse<EditCollectionResponse> { Message = "You don't own this collection." };
            }

            existingCollection = _mapper.Map<EditCollectionRequest, Collection>(editCollectionRequest, existingCollection);
            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                return new SuccessResponse<EditCollectionResponse>
                {
                    Message = "Your collection has been updated successfully.",
                    Data = _mapper.Map<Collection, EditCollectionResponse>(existingCollection)
                };
            }

            return new FailResponse<EditCollectionResponse> { Message = "Failed to update your collection." };
        }

        public async Task<BaseResponse<ViewCollectionResponse>> ViewCollectionByIdAsync(int id)
        {
            Collection? existingCollection = await _unitOfWork.Collections.GetFirstOrDefaultAsync(c => c.CollectionId == id && c.IsPrivate == false, "User");
            if (existingCollection == null)
            {
                return new NotFoundResponse<ViewCollectionResponse> { Message = "Collection not found or collection is private." };
            }
            existingCollection.Views++;
            await _unitOfWork.SaveChangesAsync();

            return new SuccessResponse<ViewCollectionResponse>
            {
                Data = _mapper.Map<Collection, ViewCollectionResponse>(existingCollection)
            };
        }

        public async Task<BaseResponse<IEnumerable<PhotoDetailsResponse>>> ViewPhotosOfCollectionAsync(int collectionId)
        {
            Collection? existingCollection = await _unitOfWork.Collections.GetCollectionIncludingPhotosByIdAsync(collectionId);
            if (existingCollection == null)
            {
                return new NotFoundResponse<IEnumerable<PhotoDetailsResponse>> { Message = "Collection not found." };
            }

            return new SuccessResponse<IEnumerable<PhotoDetailsResponse>>
            {
                Data = _mapper.Map<IEnumerable<Photo?>, IEnumerable<PhotoDetailsResponse>>(existingCollection.PhotoCollections!.Select(pc => pc.Photo).Where(p => !p.DeletedDate.HasValue))
            };
        }

        public async Task<BaseResponse<object>> DeleteCollectionAsync(int id)
        {
            Collection? existingCollection = await _unitOfWork.Collections.GetByIdAsync(id);
            if (existingCollection == null)
            {
                return new NotFoundResponse { Message = "Collection not found." };
            }

            if (existingCollection.UserId != _userService.GetCurrentUserId())
            {
                return new UnauthorizedResponse { Message = "You don't own this collection." };
            }

            _unitOfWork.Collections.Remove(existingCollection);
            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                return new SuccessResponse { Message = "Your collection has been deleted successfully." };
            }

            return new FailResponse { Message = "Something went wrong, unable to delete your collection." };
        }

        public async Task<BaseResponse<IEnumerable<ViewUserCollectionsResponse>>> ViewUserCollectionsAsync(int userId)
        {
            if ((await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.UserId == userId && !u.DeletedDate.HasValue)) == null)
            {
                return new NotFoundResponse<IEnumerable<ViewUserCollectionsResponse>> { Message = "User doesn't exist." };
            }

            return new SuccessResponse<IEnumerable<ViewUserCollectionsResponse>>
            {
                Data = _mapper.Map<IEnumerable<Collection>, IEnumerable<ViewUserCollectionsResponse>>(
                    await _unitOfWork.Collections.GetAsync(c => c.UserId == userId && !c.IsPrivate, includeProperties: "User"))
            };
        }

        public async Task<BaseResponse<IEnumerable<BasicPhotoInfoResponse>>> ViewPhotosOfUserCollectionAsync(int userId, int collectionId)
        {
            User? existingUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.UserId == userId && !u.DeletedDate.HasValue);
            if (existingUser == null)
            {
                return new NotFoundResponse<IEnumerable<BasicPhotoInfoResponse>> { Message = "User doesn't exist." };
            }

            Collection? existingCollection = await _unitOfWork.Collections.GetCollectionIncludingPhotosByIdAsync(collectionId, userId);
            if (existingCollection == null)
            {
                return new NotFoundResponse<IEnumerable<BasicPhotoInfoResponse>> { Message = "Collection not found." };
            }

            return new SuccessResponse<IEnumerable<BasicPhotoInfoResponse>>
            {
                Data = _mapper.Map<IEnumerable<Photo?>, IEnumerable<BasicPhotoInfoResponse>>(existingCollection.PhotoCollections!.Select(pc => pc.Photo))
            };
        }
    }
}
