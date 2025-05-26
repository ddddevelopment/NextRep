using Workouts.Domain.Models;

namespace Workouts.Api.Models;

public class ExerciseInfoRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = null;
    public MuscleGroup MuscleGroup { get; set; }
}