namespace Workouts.Api.Models {
    public class WorkoutCreateRequest {
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public int DurationMinutes { get; set; }
        public int CaloriesBurned { get; set; }
    }
}
