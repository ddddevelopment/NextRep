using TelegramBot.Domain.Models;

namespace TelegramBot.Domain.Services;

public interface ICommandRouter {
    Task<Result> RouteCommandAsync(TelegramMessage message, CancellationToken cancellationToken = default);
} 