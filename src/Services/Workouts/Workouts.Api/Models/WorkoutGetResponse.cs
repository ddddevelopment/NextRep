namespace Workouts.Api.Models {
    public class WorkoutGetResponse {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
        public ICollection<ExerciseDto> Exercises { get; set; } = new List<ExerciseDto>();
    }
}
