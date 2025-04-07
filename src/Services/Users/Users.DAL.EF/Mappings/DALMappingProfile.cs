using AutoMapper;
using Users.DAL.Entities;
using Users.Domain.Models;

namespace Users.DAL.Mappings {
    public class DALMappingProfile : Profile {
        public DALMappingProfile()
        {
            CreateMap<User, UserEntity>().ReverseMap();
        }
    }
}