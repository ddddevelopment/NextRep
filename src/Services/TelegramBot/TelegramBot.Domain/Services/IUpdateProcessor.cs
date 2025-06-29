using TelegramBot.Domain.Models;

namespace TelegramBot.Domain.Services;

public interface IUpdateProcessor {
    Task<Result> ProcessUpdateAsync(TelegramUpdate update, CancellationToken cancellationToken = default);
} 