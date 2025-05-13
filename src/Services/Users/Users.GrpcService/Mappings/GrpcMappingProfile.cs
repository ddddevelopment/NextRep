using AutoMapper;
using Users.Domain.Models;

namespace Users.GrpcService.Mappings {
    public class GrpcMappingProfile : Profile {
        public GrpcMappingProfile() {
            CreateMap<User, UserMessage>().ReverseMap();
        }
    }
}
