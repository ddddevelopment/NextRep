namespace Workouts.Domain.Models
{
    public class ExerciseInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public MuscleGroup MuscleGroup { get; set; }
    }
}