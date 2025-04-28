namespace Auth.Domain.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public List<Role> Roles { get; set; } = new List<Role>();
    public bool IsActive { get; set; }
}