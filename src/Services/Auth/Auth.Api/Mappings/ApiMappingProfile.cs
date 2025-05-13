using Auth.Api.Models;
using Auth.Domain.Models;
using AutoMapper;

namespace Auth.Api.Mappings {
    public class ApiMappingProfile : Profile {
        public ApiMappingProfile()
        {
            CreateMap<LoginRequest, UserLogin>();
            CreateMap<RegisterRequest, UserRegister>();
        }
    }
}