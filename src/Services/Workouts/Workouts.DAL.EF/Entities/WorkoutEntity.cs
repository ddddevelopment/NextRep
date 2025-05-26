using System.ComponentModel.DataAnnotations;

namespace Workouts.DAL.EF.Entities {
    public class WorkoutEntity
    {
        [Key]
        public Guid id { get; set; }

        [Required]
        public Guid user_id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string? Notes { get; set; } = null;
        
        [Required]
        public virtual ICollection<ExerciseEntity> Exercises { get; set; } = new List<ExerciseEntity>();
    }
}
