using Auth.Domain.Models;

namespace Auth.Domain.Repositories;

public interface IRefreshTokenRepository {
    Task SaveToken(RefreshToken token);
    Task<RefreshToken> GetToken(string token);
    Task RevokeToken(string token);
    Task<IEnumerable<RefreshToken>> GetUserTokens(Guid userId);
}