using System.ComponentModel.DataAnnotations;

namespace Workouts.DAL.EF.Entities {
    public class WorkoutEntity
    {
        [Key]
        public Guid id { get; set; }

        [Required]
        public Guid user_id { get; set; }

        [Required]
        public DateTime start_time { get; set; }

        [Required]
        public DateTime end_time { get; set; }

        public string? notes { get; set; } = null;
        
        [Required]
        public virtual ICollection<ExerciseEntity> exercises { get; set; } = new List<ExerciseEntity>();

        [Required]
        public required string name { get; set; }
    }
}
