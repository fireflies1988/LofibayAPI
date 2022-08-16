using AutoMapper;
using Common.Models.Dto.Requests;
using Domain.Models;

namespace LofibayAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddTagRequest, Tag>();
            CreateMap<SignupRequest, User>();
        }
    }
}
