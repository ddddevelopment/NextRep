namespace Auth.Domain.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string PasswordHash { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    public bool IsActive { get; set; }
    public bool RequiresTwoFactor { get; set; }
}