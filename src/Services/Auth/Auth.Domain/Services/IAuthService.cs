using Auth.Domain.Models;

namespace Auth.Domain.Services;

public interface IAuthService
{
    Task<AuthResult> Login(UserLogin user);
    Task<AuthResult> Register(UserRegister user);
}