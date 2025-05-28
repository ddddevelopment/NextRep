namespace Workouts.Api.Models;

public class ExerciseCreateRequestForWorkout
{
    public Guid ExerciseInfoId { get; set; }
    public string? Notes { get; set; } = string.Empty;
} 