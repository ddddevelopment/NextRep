using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Infrastructure.Services;

public class InMemoryUserSessionService : IUserSessionService
{
    private readonly ConcurrentDictionary<long, UserSession> _sessions = new();
    private readonly ILogger<InMemoryUserSessionService>? _logger;

    public InMemoryUserSessionService(ILogger<InMemoryUserSessionService>? logger = null)
    {
        _logger = logger;
    }

    public Task<string?> GetAuthTokenAsync(long chatId, CancellationToken cancellationToken = default)
    {
        _sessions.TryGetValue(chatId, out var session);
        return Task.FromResult(session?.AuthToken);
    }

    public Task SetAuthTokenAsync(long chatId, string authToken, CancellationToken cancellationToken = default)
    {
        _sessions.AddOrUpdate(chatId, 
            new UserSession { ChatId = chatId, AuthToken = authToken },
            (key, existing) => existing with { AuthToken = authToken });
        
        _logger?.LogInformation("Auth token set for chat: {ChatId}", chatId);
        return Task.CompletedTask;
    }

    public Task RemoveAuthTokenAsync(long chatId, CancellationToken cancellationToken = default)
    {
        if (_sessions.TryGetValue(chatId, out var session))
        {
            _sessions.TryUpdate(chatId, session with { AuthToken = null }, session);
            _logger?.LogInformation("Auth token removed for chat: {ChatId}", chatId);
        }
        
        return Task.CompletedTask;
    }

    public Task<bool> IsUserAuthenticatedAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var hasToken = _sessions.TryGetValue(chatId, out var session) && !string.IsNullOrEmpty(session.AuthToken);
        return Task.FromResult(hasToken);
    }

    public Task<UserDto?> GetUserInfoAsync(long chatId, CancellationToken cancellationToken = default)
    {
        _sessions.TryGetValue(chatId, out var session);
        return Task.FromResult(session?.UserInfo);
    }

    public Task SetUserInfoAsync(long chatId, UserDto user, CancellationToken cancellationToken = default)
    {
        _sessions.AddOrUpdate(chatId,
            new UserSession { ChatId = chatId, UserInfo = user },
            (key, existing) => existing with { UserInfo = user });
        
        _logger?.LogInformation("User info set for chat: {ChatId}, User: {Email}", chatId, user.Email);
        return Task.CompletedTask;
    }

    private record UserSession
    {
        public long ChatId { get; init; }
        public string? AuthToken { get; init; }
        public UserDto? UserInfo { get; init; }
    }
} 