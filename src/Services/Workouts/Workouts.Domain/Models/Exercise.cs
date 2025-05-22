namespace Workouts.Domain.Models;

public class Exercise
{
    public Guid Id { get; set; }
    public ExerciseInfo ExerciseInfo { get; set; } = new ExerciseInfo();
    public string Name { get; set; } = string.Empty;
    public ICollection<Set> Sets { get; set; } = new List<Set>();
    public Workout Workout { get; set; } = null!;
}