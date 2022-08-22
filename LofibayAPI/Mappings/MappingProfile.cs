using AutoMapper;
using Domain.Models.DTOs.Requests;
using Domain.Entities;
using Domain.Models.DTOs.Responses;

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
            CreateMap<UpdateUserAddressRequest, UserAddress>().ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null)); 
            CreateMap<UpdateUserRequest, User>().ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Photo?, UploadPhotoResponse>();
            CreateMap<Photo?, PhotoDetailsResponse>();
            CreateMap<UpdatePhotoRequest, Photo>().ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Photo, UpdatePhotoResponse>();
        }
    }
}
