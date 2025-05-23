using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Workouts.Domain.Models;

namespace Workouts.DAL.Entities
{
    public class ExerciseEntity
    {
        [Key]
        public Guid id { get; set; }

        [Required]
        public string? description { get; set; }

        [ForeignKey("exercise_info_id")]
        public virtual ExerciseInfoEntity exercise_info { get; set; } = null!;

        [ForeignKey("workout_id")]
        public virtual WorkoutEntity workout { get; set; } = null!;

        public virtual ICollection<SetEntity> sets { get; set; } = new List<SetEntity>();
    }
}
