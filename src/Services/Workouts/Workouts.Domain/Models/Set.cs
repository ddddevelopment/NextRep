namespace Workouts.Domain.Models;

public class Set
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public int Reps { get; set; }
    public int Weight { get; set; }
    public string? Notes { get; set; } = null;
}
