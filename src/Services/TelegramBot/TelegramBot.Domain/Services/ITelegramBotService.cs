using TelegramBot.Domain.Models;

namespace TelegramBot.Domain.Services;

public interface ITelegramBotService {
    Task<Result> SendMessageAsync(long chatId, string text, object? replyMarkup = null, CancellationToken cancellationToken = default);
    Task<Result> SetWebhookAsync(string url, CancellationToken cancellationToken = default);
} 