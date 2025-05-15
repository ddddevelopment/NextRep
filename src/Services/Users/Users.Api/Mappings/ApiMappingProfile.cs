using AutoMapper;
using Users.Api.Models;
using Users.Domain.Models;

namespace Users.Api.Mappings {
    public class ApiMappingProfile : Profile {
        public ApiMappingProfile()
        {
            CreateMap<UserCreateRequest, User>();
            CreateMap<User, UserGetResponse>();
            CreateMap<UserUpdateDto, User>().ReverseMap();
        }
    }
}