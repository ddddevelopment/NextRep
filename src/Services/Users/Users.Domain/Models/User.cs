using System.ComponentModel.DataAnnotations;

namespace Users.Domain.Models {
    public class User {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
    }
}