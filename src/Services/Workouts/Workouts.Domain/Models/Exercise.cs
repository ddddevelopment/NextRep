namespace Workouts.Domain.Models;

public class Exercise
{
    public Guid Id { get; set; }
    public Guid WorkoutId { get; set; }
    public string Name { get; set; }
    public ICollection<Set> Sets { get; set; } = new List<Set>();
}