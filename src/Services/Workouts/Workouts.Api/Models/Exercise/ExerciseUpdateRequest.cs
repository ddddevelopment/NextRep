namespace Workouts.Api.Models;

public class ExerciseUpdateRequest
{
    public Guid ExerciseInfoId { get; set; }
    public string? Notes { get; set; }
}
