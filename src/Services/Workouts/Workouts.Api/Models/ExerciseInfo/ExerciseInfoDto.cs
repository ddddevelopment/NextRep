using Workouts.Domain.Models;

namespace Workouts.Api.Models;

public class ExerciseInfoDto : ExerciseInfoBase
{
    public Guid Id { get; set; }
}