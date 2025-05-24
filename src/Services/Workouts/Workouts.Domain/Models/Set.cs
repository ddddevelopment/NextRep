namespace Workouts.Domain.Models;

public class Set
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Reps { get; set; }
    public int Weight { get; set; }
    public string? Notes { get; set; } = null;
    public Exercise Exercise { get; set; } = null!;
}
