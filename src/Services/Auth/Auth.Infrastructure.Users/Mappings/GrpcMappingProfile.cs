using Auth.Domain.Models;
using AutoMapper;

namespace Auth.Infrastructure.Users.Mappings {
    public class GrpcMappingProfile : Profile {
        public GrpcMappingProfile() {
            CreateMap<GetUserByEmailResponse, UserDto>();
        }
    }
}
