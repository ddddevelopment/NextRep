using AutoMapper;
using Users.DAL.Entities;
using Users.Domain.Models;

namespace Users.DAL.Mappings {
    public class DALMappingProfile : Profile {
        public DALMappingProfile()
        {
            CreateMap<User, UserEntity>()
                .ForMember(dest => dest.password_hash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.is_active, opt => opt.MapFrom(src => src.IsActive))
                .ReverseMap();
        }
    }
}