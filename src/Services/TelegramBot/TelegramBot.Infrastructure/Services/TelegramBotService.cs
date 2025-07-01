using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;
using TelegramBot.Infrastructure.Settings;

namespace TelegramBot.Infrastructure.Services;

public class TelegramBotService : ITelegramBotService {
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramBotService>? _logger;

    public TelegramBotService(ITelegramBotClient botClient, ILogger<TelegramBotService>? logger = null) {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task<Result> SendMessageAsync(long chatId, string text, object? replyMarkup = null, CancellationToken cancellationToken = default) {
        try {
            _logger?.LogInformation("Sending message to chat {ChatId}", chatId);
            
            await _botClient.SendMessage(
                chatId,
                text,
                parseMode: ParseMode.Markdown,
                replyMarkup: replyMarkup as Telegram.Bot.Types.ReplyMarkups.ReplyMarkup,
                cancellationToken: cancellationToken);
            
            _logger?.LogInformation("Message sent successfully to chat {ChatId}", chatId);
            return Result.Success();
        }
        catch (Exception ex) {
            _logger?.LogError(ex, "Error sending message to chat {ChatId}", chatId);
            return Result.Failure($"Error sending message: {ex.Message}");
        }
    }

    public async Task<Result> SetWebhookAsync(string url, CancellationToken cancellationToken = default) {
        try {
            _logger?.LogInformation("Setting webhook to {Url}", url);
            
            await _botClient.SetWebhook(url, cancellationToken: cancellationToken);
            
            _logger?.LogInformation("Webhook set successfully to {Url}", url);
            return Result.Success();
        }
        catch (Exception ex) {
            _logger?.LogError(ex, "Error setting webhook to {Url}", url);
            return Result.Failure($"Error setting webhook: {ex.Message}");
        }
    }
} 