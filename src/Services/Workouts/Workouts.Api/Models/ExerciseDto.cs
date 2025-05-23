using Workouts.Domain.Models;

namespace Workouts.Api.Models;

public class ExerciseDto
{
    public Guid Id { get; set; }
    public ExerciseInfo ExerciseInfo { get; set; } = new ExerciseInfo();
    public string Name { get; set; } = string.Empty;
    public ICollection<SetDto> Sets { get; set; } = new List<SetDto>();
    public Guid WorkoutId { get; set; }
}
