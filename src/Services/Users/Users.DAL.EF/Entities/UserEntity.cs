using System.ComponentModel.DataAnnotations;
using Users.Domain.Models;

namespace Users.DAL.Entities {
    public class UserEntity {
        [Key]
        public Guid id { get; set; }

        [Required(ErrorMessage = "name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "name length must be greater than 2 and less than 50")]
        public string name { get; set; }

        [Required(ErrorMessage = "email is required")]
        [EmailAddress(ErrorMessage = "email must be a valid email address")]
        public string email { get; set; }

        [Required(ErrorMessage = "telephone is required")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "telephone must be a valid phone number")]
        public string telephone { get; set; }

        [Required(ErrorMessage = "password_hash is required")]
        public string password_hash { get; set; }

        [Required(ErrorMessage = "roles is required")]
        public List<Role> roles { get; set; } = new List<Role>();

        [Required(ErrorMessage = "is_active is required")]
        public bool is_active { get; set; }
    }
}