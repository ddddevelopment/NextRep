using AutoMapper;
using Users.Api.Models;
using Users.Domain.Models;

namespace Users.Api.Mappings {
    public class ApiMappingProfile : Profile {
        public ApiMappingProfile()
        {
            CreateMap<UserCreateDto, User>();
            CreateMap<User, UserGetDto>();
            CreateMap<UserUpdateDto, User>().ReverseMap();
        }
    }
}