namespace Users.Api.Models {
    public class UserGetDto {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
    }
}