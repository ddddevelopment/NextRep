using System.ComponentModel.DataAnnotations;
using Workouts.Domain.Models;

namespace Workouts.DAL.Entities {
    public class WorkoutEntity {
        [Key]
        public Guid id { get; set; }
        [Required]
        public Guid user_id { get; set; }
        [Required]
        public DateTime date { get; set; }
        [Required]
        [StringLength(50)]
        public string type { get; set; }
        [Required]
        public int duration_minutes { get; set; }
        [Required]
        public int calories_burned { get; set; }
    }
}
