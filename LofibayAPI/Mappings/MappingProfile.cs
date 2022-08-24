using AutoMapper;
using Domain.Models.DTOs.Requests;
using Domain.Entities;
using Domain.Models.DTOs.Requests.Users;
using Domain.Models.DTOs.Requests.Photos;
using Domain.Models.DTOs.Requests.Collections;
using Domain.Models.DTOs.Responses.Users;
using Domain.Models.DTOs.Responses.Collections;
using Domain.Models.DTOs.Responses.Photos;

namespace LofibayAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);

            CreateMap<AddTagRequest, Tag>();

            CreateMap<SignupRequest, User>();
            CreateMap<User, UserInfoResponse>();
            CreateMap<User, BasicUserInfoResponse>();
            CreateMap<UpdateUserAddressRequest, UserAddress>().ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null)); 
            CreateMap<UpdateUserRequest, User>().ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Photo?, UploadPhotoResponse>();
            CreateMap<Photo?, PhotoDetailsResponse>();
            CreateMap<UpdatePhotoRequest, Photo>().ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Photo, UpdatePhotoResponse>();
            CreateMap<Photo?, CurrentUserPhotoDetailsResponse>();
            CreateMap<Photo?, ViewYourLikedPhotosResponse>();
            CreateMap<Photo, ViewYourUploadedPhotosResponse>();

            CreateMap<CreateCollectionRequest, Collection>();
            CreateMap<Collection, CreateCollectionResponse>();
            CreateMap<EditCollectionRequest, Collection>().ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Collection, EditCollectionResponse>();
            CreateMap<Collection, ViewCollectionResponse>();
            CreateMap<Collection, CurrentUserCollectionResponse>();
        }
    }
}
