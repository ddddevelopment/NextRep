using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Workouts.DAL.EF.Entities
{
    public class ExerciseEntity
    {
        [Key]
        public Guid id { get; set; }

        public Guid exercise_info_id { get; set; }

        [ForeignKey(nameof(exercise_info_id))]
        public virtual ExerciseInfoEntity exercise_info { get; set; } = null!;

        public Guid workout_id { get; set; }

        [ForeignKey(nameof(workout_id))]
        public virtual WorkoutEntity workout { get; set; } = null!;

        public virtual ICollection<SetEntity> sets { get; set; } = new List<SetEntity>();

        public string? notes { get; set; } = string.Empty;
    }
}
