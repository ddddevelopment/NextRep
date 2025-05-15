using Users.Domain.Models;

namespace Users.Api.Models {
    public class UserCreateRequest {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
    }
}