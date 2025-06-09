namespace Auth.Domain.Models;

public class UserCreateDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Telephone { get; set; }
}