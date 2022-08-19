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
            CreateMap<AddTagRequest, Tag>();
            CreateMap<SignupRequest, User>();
            CreateMap<User, UserInfoResponse>();
            CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<UpdateUserAddressRequest, UserAddress>().ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null)); 
            CreateMap<UpdateUserRequest, User>().ForAllMembers(options => options.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
