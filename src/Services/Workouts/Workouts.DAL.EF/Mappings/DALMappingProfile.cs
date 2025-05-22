using AutoMapper;
using Workouts.DAL.Entities;
using Workouts.Domain.Models;

namespace Workouts.DAL.Mappings {
    public class DALMappingProfile : Profile {
        public DALMappingProfile()
        {
            CreateMap<Workout, WorkoutEntity>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.user_id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.duration_minutes, opt => opt.MapFrom(src => src.DurationMinutes))
                .ReverseMap();
        }
    }
}
