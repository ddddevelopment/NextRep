namespace Workouts.Api.Models;

public abstract class WorkoutBase
{
    public Guid UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Notes { get; set; }
}
