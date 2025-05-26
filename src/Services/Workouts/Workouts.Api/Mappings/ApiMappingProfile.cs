using AutoMapper;
using Workouts.Api.Models;

using Workouts.Domain.Models;

namespace Workouts.Api.Mappings {
    public class ApiMappingProfile : Profile {
        public ApiMappingProfile()
        {
            CreateMap<WorkoutCreateRequest, Workout>();
            CreateMap<Workout, WorkoutGetResponse>();
            CreateMap<WorkoutUpdateDto, Workout>().ReverseMap();
            CreateMap<ExerciseDto, Exercise>().ReverseMap();
            CreateMap<SetDto, Set>().ReverseMap();
            CreateMap<ExerciseInfoRequest, ExerciseInfo>();
        }
    }
}