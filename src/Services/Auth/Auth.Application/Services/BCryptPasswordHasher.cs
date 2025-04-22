using Auth.Domain.Services;
namespace Auth.Application.Services;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool VerifyPassword(string password, string hash)
    {
        try {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (Exception exception) { 
            return false;
        }
    }
}
