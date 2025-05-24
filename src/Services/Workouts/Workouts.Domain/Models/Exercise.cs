namespace Workouts.Domain.Models;

public class Exercise
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ExerciseInfoId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Set> Sets { get; set; } = new List<Set>();
    public Guid WorkoutId { get; set; }
}