<<<<<<< HEAD
namespace Auth.Domain.Services;

public interface IAuthService {
    Task<string> GenerateToken(string email);
=======
using Auth.Domain.Models;

namespace Auth.Domain.Services;

public interface IAuthService
{
    Task<AuthResult> Authenticate(UserDto user, string password);
>>>>>>> users
}