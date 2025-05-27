namespace Workouts.Api.Models;

public abstract class ExerciseInfoBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = null;
    public string MuscleGroup { get; set; } = string.Empty;
}