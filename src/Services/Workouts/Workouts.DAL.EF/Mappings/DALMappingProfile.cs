using AutoMapper;
using Workouts.DAL.EF.Entities;
using Workouts.Domain.Models;

namespace Workouts.DAL.EF.Mappings {
    public class DALMappingProfile : Profile {
        public DALMappingProfile()
        {
            CreateMap<Workout, WorkoutEntity>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.user_id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.Exercises, opt => opt.MapFrom(src => src.Exercises))
                .ReverseMap();

            CreateMap<Set, SetEntity>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.reps, opt => opt.MapFrom(src => src.Reps))
                .ForMember(dest => dest.weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.notes, opt => opt.MapFrom(src => src.Notes))
                .ReverseMap();

            CreateMap<Exercise, ExerciseEntity>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.sets, opt => opt.MapFrom(src => src.Sets))
                .ReverseMap();

            CreateMap<ExerciseInfo, ExerciseInfoEntity>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.muscle_group, opt => opt.MapFrom(src => src.MuscleGroup))
                .ReverseMap();
        }
    }
}
