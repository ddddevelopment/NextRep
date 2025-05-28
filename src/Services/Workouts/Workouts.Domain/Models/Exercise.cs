namespace Workouts.Domain.Models;

public class Exercise
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ExerciseInfoId { get; set; }
    public ICollection<Set> Sets { get; set; } = new List<Set>();
    public Guid WorkoutId { get; set; }
    public string? Notes { get; set; } = string.Empty;
}
