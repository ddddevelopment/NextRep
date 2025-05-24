namespace Workouts.Api.Models;

public class SetDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Reps { get; set; }
    public int Weight { get; set; }
    public string? Notes { get; set; }
    public Guid ExerciseId { get; set; }
}
