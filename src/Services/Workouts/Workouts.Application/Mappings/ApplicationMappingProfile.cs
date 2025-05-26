using AutoMapper;
using Workouts.Application.Commands.Workouts.CreateWorkout;
using Workouts.Domain.Models;

namespace Workouts.Application.Mappings;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<CreateWorkoutCommand, Workout>().ReverseMap();
    }
}