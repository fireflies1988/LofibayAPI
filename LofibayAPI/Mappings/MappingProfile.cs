using AutoMapper;
using Domain.Models.DTOs.Requests;
using Domain.Entities;

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
