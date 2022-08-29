using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Common.Helpers;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Services;
using Domain.Models.DTOs.Requests.Photos;
using Domain.Models.DTOs.Responses.Photos;
using Domain.Models.ResponseTypes;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace DataAccessEF.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IUserService _userService;
        private readonly Cloudinary _cloudinary;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PhotoService(IUserService userService, Cloudinary cloudinary, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userService = userService;
            _cloudinary = cloudinary;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<object>> DeletePhotoAsync(int id)
        {
            Photo? existingPhoto = await _unitOfWork.Photos.GetFirstOrDefaultAsync(p => p.PhotoId == id && !p.DeletedDate.HasValue);
            if (existingPhoto == null)
            {
                return new NotFoundResponse { Message = "Photo not found." };
            }

            if (existingPhoto.UserId != _userService.GetCurrentUserId())
            {
                return new UnauthorizedResponse { Message = "You don't own this photo." };
            }

            existingPhoto.DeletedDate = DateTime.Now;
            if (await _unitOfWork.SaveChangesAsync() > 0)
            {
                return new SuccessResponse { Message = "Your photo has been deleted." };
            }

            return new FailResponse { Message = "Unable to delete your photo." };
        }

        public async Task<BaseResponse<PhotoDetailsResponse>> GetPhotoDetailsByIdAsync(int id)
        {
            Photo? photo = await _unitOfWork.Photos.GetPhotoDetailsByIdAsync(id);
            if (photo == null)
            {
                return new NotFoundResponse<PhotoDetailsResponse> { Message = "Photo not found." };
            }
            photo.Views++;
            await _unitOfWork.SaveChangesAsync();

            PhotoDetailsResponse response = _mapper.Map<Photo?, PhotoDetailsResponse>(photo);
            response.Likes = photo.LikedPhotos?.Count() ?? 0;
            return new SuccessResponse<PhotoDetailsResponse>
            {
                Data = response
            };
        }

        public async Task InsertTagsAsync(Photo photo, IList<string>? tags)
        {
            IList<PhotoTag> photoTags = new List<PhotoTag>();
            for (int i = 0; i < tags?.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(tags[i]))
                {
                    continue;
                }
                // standardize the tag
                string tag = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tags[i].Trim().ToLower());
                tag = Regex.Replace(tag, @"\s+", " ");

                Tag? existingTag = await _unitOfWork.Tags.GetByIdAsync(tag);
                if (existingTag == null)
                {
                    Tag newTag = new Tag { Name = tag };
                    await _unitOfWork.Tags.AddAsync(newTag);
                    if (await _unitOfWork.SaveChangesAsync() > 0)
                    {
                        photoTags.Add(new PhotoTag
                        {
                            PhotoId = photo.PhotoId,
                            TagName = newTag.Name
                        });
                    }
                }
                else
                {
                    photoTags.Add(new PhotoTag
                    {
                        PhotoId = photo.PhotoId,
                        TagName = existingTag?.Name
                    });
                }
            }
            photo.PhotoTags = photoTags.GroupBy(pt => new { pt.PhotoId, pt.TagName }).Select(pt => pt.First()).ToList(); // assign distinct elements only
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<BaseResponse<object>> LikeOrUnlikePhotoAsync(int id)
        {
            Photo? existingPhoto = await _unitOfWork.Photos.GetFirstOrDefaultAsync(p => p.PhotoId == id && !p.DeletedDate.HasValue);
            if (existingPhoto == null)
            {
                return new NotFoundResponse { Message = "Photo not found." };
            }

            User? currentUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.UserId == _userService.GetCurrentUserId(), "LikedPhotos");
            var existingLikedPhoto = currentUser?.LikedPhotos?.FirstOrDefault(lp => lp.PhotoId == id);
            if (existingLikedPhoto == null)
            {
                // like
                currentUser?.LikedPhotos?.Add(new LikedPhoto
                {
                    UserId = currentUser.UserId,
                    PhotoId = id
                });

                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return new SuccessResponse { Message = "You liked this photo." };
                }
            }
            else
            {
                // unlike
                currentUser?.LikedPhotos?.Remove(existingLikedPhoto);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return new SuccessResponse { Message = "You unliked this photo." };
                }
            }

            return new FailResponse { Message = "Something went wrong, unable to like or unlike this photo." };
        }

        public async Task<BaseResponse<UpdatePhotoResponse>> UpdatePhotoAsync(int id, UpdatePhotoRequest updatePhotoRequest)
        {
            Photo? existingPhoto = await _unitOfWork.Photos.GetFirstOrDefaultAsync(p => p.PhotoId == id && !p.DeletedDate.HasValue, "PhotoTags");
            if (existingPhoto == null)
            {
                return new NotFoundResponse<UpdatePhotoResponse> { Message = "Photo not found" };
            }

            if (existingPhoto.UserId != _userService.GetCurrentUserId())
            {
                return new UnauthorizedResponse<UpdatePhotoResponse> { Message = "You don't own this photo." };
            }

            existingPhoto = _mapper.Map<UpdatePhotoRequest, Photo>(updatePhotoRequest, existingPhoto);

            // insert the input tags into Tags and PhotoTags
            await InsertTagsAsync(existingPhoto, updatePhotoRequest.Tags);

            return new SuccessResponse<UpdatePhotoResponse>
            {
                Message = "Your photo has been updated successfully.",
                Data = _mapper.Map<Photo, UpdatePhotoResponse>(existingPhoto)
            };
        }

        public async Task<BaseResponse<object>> UploadPhotoAsync(UploadPhotoRequest uploadPhotoRequest)
        {
            ImageUploadParams uploadParams = new ImageUploadParams
            {
                File = new FileDescription(_userService.GetCurrentUserId().ToString(), uploadPhotoRequest.ImageFile!.OpenReadStream()),
                Folder = $"{ConfigurationHelper.Configuration!["CloudinaryFolder"]}/{_userService.GetCurrentUserId()}",
                Faces = true,
                Colors = true,
                Phash = true,
                ImageMetadata = true
            };
            ImageUploadResult uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == HttpStatusCode.OK)
            {
                using var transaction = await _unitOfWork.Database.BeginTransactionAsync();
                try
                {
                    // insert the photo into Photos
                    Photo newPhoto = new Photo
                    {
                        PhotoUrl = uploadResult.SecureUrl.ToString(),
                        PublicId = uploadResult.PublicId,
                        Description = uploadPhotoRequest.Description,
                        Location = uploadPhotoRequest.Location,
                        TakenAt = uploadPhotoRequest.TakenAt,
                        Width = uploadResult.Width,
                        Height = uploadResult.Height,
                        FileSize = uploadResult.Bytes,
                        Format = uploadResult.Format,
                        Camera = uploadPhotoRequest.Camera,
                        Software = uploadPhotoRequest.Software,
                        FacesDetected = uploadResult.Faces.Count(),
                        Phash = uploadResult.Phash,
                        SemiTransparent = uploadResult.SemiTransparent,
                        Grayscale = uploadResult.Grayscale,
                        UserId = _userService.GetCurrentUserId(),
                    };
                    await _unitOfWork.Photos.AddAsync(newPhoto);
                    if (await _unitOfWork.SaveChangesAsync() == 0)
                    {
                        await _cloudinary.DestroyAsync(new DeletionParams(uploadResult.PublicId));
                        return new FailResponse
                        {
                            Message = "Failed to save your photo to the database.",
                        };
                    }

                    // insert the input tags into Tags and PhotoTags
                    await InsertTagsAsync(newPhoto, uploadPhotoRequest.Tags);

                    #region Insert the colors analyzed by Google and Cloudinary after uploading into Colors and PhotoColors
                    List<Tuple<string, float>> predominantColorsByGoogle = 
                        uploadResult.Predominant.Google.Select(c => new Tuple<string, float>(c[0].ToString()!, float.Parse(c[1].ToString()!))).ToList();
                    List<Tuple<string, float>> predominantColorsByCloudinary =
                        uploadResult.Predominant.Cloudinary.Select(c => new Tuple<string, float>(c[0].ToString()!, float.Parse(c[1].ToString()!))).ToList();
                    IList<PhotoColor> photoColors = new List<PhotoColor>();

                    for (int i = 0; i < predominantColorsByGoogle.Count; i++)
                    {
                        string color = predominantColorsByGoogle[i].Item1.Trim();
                        Color? existingColor = await _unitOfWork.Colors.GetByIdAsync(color);
                        if (existingColor == null)
                        {
                            Color newColor = new Color { Name = color };
                            await _unitOfWork.Colors.AddAsync(newColor);
                            if (await _unitOfWork.SaveChangesAsync() > 0)
                            {
                                photoColors.Add(new PhotoColor
                                {
                                    PhotoId = newPhoto.PhotoId,
                                    ColorName = newColor.Name,
                                    ColorAnalyzerId = ColorAnalyzers.Google,
                                    PredominantPercent = predominantColorsByGoogle[i].Item2
                                });
                            }
                        }
                        else
                        {
                            photoColors.Add(new PhotoColor
                            {
                                PhotoId = newPhoto.PhotoId,
                                ColorName = existingColor.Name,
                                ColorAnalyzerId = ColorAnalyzers.Google,
                                PredominantPercent = predominantColorsByGoogle[i].Item2
                            });
                        }
                    }

                    for (int i = 0; i < predominantColorsByCloudinary.Count; i++)
                    {
                        string color = predominantColorsByCloudinary[i].Item1.Trim();
                        Color? existingColor = await _unitOfWork.Colors.GetByIdAsync(color);
                        if (existingColor == null)
                        {
                            Color newColor = new Color { Name = color };
                            await _unitOfWork.Colors.AddAsync(newColor);
                            if (await _unitOfWork.SaveChangesAsync() > 0)
                            {
                                photoColors.Add(new PhotoColor
                                {
                                    PhotoId = newPhoto.PhotoId,
                                    ColorName = newColor.Name,
                                    ColorAnalyzerId = ColorAnalyzers.Cloudinary,
                                    PredominantPercent = predominantColorsByCloudinary[i].Item2
                                });
                            }
                        }
                        else
                        {
                            photoColors.Add(new PhotoColor
                            {
                                PhotoId = newPhoto.PhotoId,
                                ColorName = existingColor.Name,
                                ColorAnalyzerId = ColorAnalyzers.Cloudinary,
                                PredominantPercent = predominantColorsByCloudinary[i].Item2
                            });
                        }
                    }

                    newPhoto.PhotoColors = photoColors;
                    await _unitOfWork.SaveChangesAsync();
                    #endregion

                    await transaction.CommitAsync();
                    return new SuccessResponse
                    {
                        Message = "Your photo has been uploaded successfully.",
                        Data = _mapper.Map<Photo?, UploadPhotoResponse>(await _unitOfWork.Photos.GetPhotoIncludingColorAnalyzerByIdAsync(newPhoto.PhotoId))
                    };
                }
                catch
                {
                    await _cloudinary.DestroyAsync(new DeletionParams(uploadResult.PublicId));
                    await transaction.RollbackAsync();
                    throw;
                }
            } 
            else
            {
                return new FailResponse
                {
                    Message = "Failed to upload your photo.",
                    Data = uploadResult
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<BasicPhotoInfoResponse>>> ViewLikedPhotosOfUserAsync(int id)
        {
            User? existingUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.UserId == id && !u.DeletedDate.HasValue);
            if (existingUser == null)
            {
                return new NotFoundResponse<IEnumerable<BasicPhotoInfoResponse>> { Message = "User doesn't exist." };
            }

            var likedPhotos = await _unitOfWork.LikedPhotos.GetPhotosThatUserLikedAsync(id);
            return new SuccessResponse<IEnumerable<BasicPhotoInfoResponse>>
            {
                Data = _mapper.Map<IEnumerable<Photo?>, IEnumerable<BasicPhotoInfoResponse>>(likedPhotos)
            };
        }

        public async Task<BaseResponse<IEnumerable<BasicPhotoInfoResponse>>> ViewUserUploadedPhotosAsync(int id)
        {
            User? existingUser = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.UserId == id && !u.DeletedDate.HasValue);
            if (existingUser == null)
            {
                return new NotFoundResponse<IEnumerable<BasicPhotoInfoResponse>> { Message = "User doesn't exist." };
            }

            var userUploadedPhotos = await _unitOfWork.Photos.GetAsync(p => p.UserId == id && !p.DeletedDate.HasValue, includeProperties: "User,LikedPhotos");
            return new SuccessResponse<IEnumerable<BasicPhotoInfoResponse>>
            {
                Data = _mapper.Map<IEnumerable<Photo>, IEnumerable<BasicPhotoInfoResponse>>(userUploadedPhotos)
            };
        }

        public async Task<BaseResponse<IEnumerable<ViewYourLikedPhotosResponse>>> ViewYourLikedPhotoAsync()
        {
            var likedPhotos = (await _unitOfWork.LikedPhotos.GetAsync(lp => lp.UserId == _userService.GetCurrentUserId(), includeProperties: "Photo,User")).Select(lp => lp.Photo).Where(p => !p.DeletedDate.HasValue);
            return new SuccessResponse<IEnumerable<ViewYourLikedPhotosResponse>>
            {
                Data = _mapper.Map<IEnumerable<Photo?>, IEnumerable<ViewYourLikedPhotosResponse>>(likedPhotos)
            };
        }

        public async Task<BaseResponse<IEnumerable<ViewYourUploadedPhotosResponse>>> ViewYourUploadedPhotosAysnc()
        {
            var yourUploadedPhotos = await _unitOfWork.Photos.GetAsync(p => p.UserId == _userService.GetCurrentUserId() && !p.DeletedDate.HasValue, includeProperties: "User");
            return new SuccessResponse<IEnumerable<ViewYourUploadedPhotosResponse>> {
                Data = _mapper.Map<IEnumerable<Photo>, IEnumerable<ViewYourUploadedPhotosResponse>>(yourUploadedPhotos)
            };
        }
    }
}
