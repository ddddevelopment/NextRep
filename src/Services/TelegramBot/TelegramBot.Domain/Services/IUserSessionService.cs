using TelegramBot.Domain.Models;

namespace TelegramBot.Domain.Services;

public interface IUserSessionService
{
    Task<string?> GetAuthTokenAsync(long chatId, CancellationToken cancellationToken = default);
    Task SetAuthTokenAsync(long chatId, string authToken, CancellationToken cancellationToken = default);
    Task RemoveAuthTokenAsync(long chatId, CancellationToken cancellationToken = default);
    Task<bool> IsUserAuthenticatedAsync(long chatId, CancellationToken cancellationToken = default);
    
    Task<UserDto?> GetUserInfoAsync(long chatId, CancellationToken cancellationToken = default);
    Task SetUserInfoAsync(long chatId, UserDto user, CancellationToken cancellationToken = default);
} 