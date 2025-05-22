using System.ComponentModel.DataAnnotations;
using Workouts.Domain.Models;

namespace Workouts.DAL.Entities
{
    public class ExerciseInfoEntity
    {
        [Key]
        public Guid id { get; set; }

        [Required]
        public string name { get; set; } = string.Empty;

        public string? description { get; set; }
        
        [Required]
        public MuscleGroup muscle_group { get; set; } 
    }
}
