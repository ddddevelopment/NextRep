namespace Auth.Domain.Services;

public interface IAuthService {
    Task<string> GenerateToken(string email);
}