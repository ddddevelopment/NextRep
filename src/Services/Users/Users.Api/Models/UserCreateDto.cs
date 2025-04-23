using Users.Domain.Models;

namespace Users.Api.Models {
    public class UserCreateDto {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string PasswordHash { get; set; }
        public List<Role> Roles { get; set; } = new List<Role>();
        public bool IsActive { get; set; } = true;
    }
}