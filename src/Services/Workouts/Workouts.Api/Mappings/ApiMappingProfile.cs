using AutoMapper;
using Workouts.Api.Models;

using Workouts.Domain.Models;

namespace Workouts.Api.Mappings {
    public class ApiMappingProfile : Profile {
        public ApiMappingProfile()
        {
            CreateMap<WorkoutCreateRequest, Workout>();
            CreateMap<WorkoutDto, Workout>().ReverseMap();
            CreateMap<ExerciseDto, Exercise>().ReverseMap();
            CreateMap<SetDto, Set>().ReverseMap();
            CreateMap<ExerciseInfoCreateRequest, ExerciseInfo>();
            CreateMap<ExerciseInfoDto, ExerciseInfo>()
                .ForMember(dest => dest.MuscleGroup, opt => opt.MapFrom(src => Enum.Parse<MuscleGroup>(src.MuscleGroup, true)))
                .ReverseMap()
                .ForMember(dest => dest.MuscleGroup, opt => opt.MapFrom(src => src.MuscleGroup.ToString()));
        }
    }
}