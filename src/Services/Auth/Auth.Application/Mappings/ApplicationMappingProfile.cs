using Auth.Domain.Models;
using AutoMapper;

namespace Auth.Application.Mappings;

public class ApplicationMappingProfile : Profile {
    public ApplicationMappingProfile()
    {
        CreateMap<UserRegister, UserDto>();
    }
}