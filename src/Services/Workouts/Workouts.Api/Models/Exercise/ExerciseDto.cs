using Workouts.Domain.Models;

namespace Workouts.Api.Models;

public class ExerciseDto
{
    public Guid Id { get; set; }
    public Guid WorkoutId { get; set; }
    public Guid ExerciseInfoId { get; set; } 
    public string Name { get; set; } = string.Empty; 
    public ICollection<SetDto> Sets { get; set; } = new List<SetDto>();
}
