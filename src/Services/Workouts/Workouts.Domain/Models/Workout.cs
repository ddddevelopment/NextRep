namespace Workouts.Domain.Models {
    public class Workout
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public int DurationMinutes { get; set; }
        public string Notes { get; set; }
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
