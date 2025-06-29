namespace Workouts.Api.Models;

public abstract class WorkoutBase
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Notes { get; set; }
}
