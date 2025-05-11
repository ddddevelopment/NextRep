using Auth.Domain.Models;

namespace Auth.Domain.Services;

public interface IAuthService
{
    Task<AuthResult> Authenticate(UserDto user, string password);
}