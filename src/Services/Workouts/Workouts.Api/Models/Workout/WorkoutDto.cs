namespace Workouts.Api.Models;

public class WorkoutDto : WorkoutBase
{
    public Guid Id { get; set; }
    public ICollection<ExerciseDto> Exercises { get; set; } = new List<ExerciseDto>();
}
