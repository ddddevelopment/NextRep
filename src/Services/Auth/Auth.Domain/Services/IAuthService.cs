using Auth.Domain.Models;

namespace Auth.Domain.Services;

public interface IAuthService {
    Task<AuthResult> Authenticate(UserDto user, string password);
    Task<AuthResult> RefreshToken(string refreshToken);
    Task<TokenValidationResult> ValidateToken(string token);
    Task RevokeTokenAsync(string refreshToken);
}