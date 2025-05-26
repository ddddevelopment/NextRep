using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Workouts.DAL.EF.Entities
{
    public class SetEntity
    {
        [Key]
        public Guid id { get; set; }

        [Required]
        public int reps { get; set; }

        [Required]
        public int weight { get; set; }

        public string? notes { get; set; }

        [ForeignKey("exercise_id")]
        public virtual ExerciseEntity exercise { get; set; } = null!;
    }
}
