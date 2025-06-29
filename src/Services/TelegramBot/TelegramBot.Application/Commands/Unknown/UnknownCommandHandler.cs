using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Unknown;

public class UnknownCommandHandler : BaseTelegramCommandHandler<UnknownCommand> {
    public UnknownCommandHandler(ITelegramBotService botService, ILogger<UnknownCommandHandler>? logger = null) 
        : base(botService, logger) {
    }

    protected override async Task<Result> ExecuteAsync(UnknownCommand request, CancellationToken cancellationToken) {
        Logger?.LogInformation("Handling unknown command for user {UserId}: {Text}", request.Message.From.Id, request.Message.Text);
        
        var responseMessage = "🤔 Извините, я не понимаю эту команду.\n\n" +
                            "Используйте:\n" +
                            "📋 /menu - Главное меню\n" +
                            "❓ /help - Список всех команд\n\n" +
                            "Или просто отправьте /start для начала работы.";
        
        return await BotService.SendMessageAsync(request.Message.ChatId, responseMessage, cancellationToken);
    }
} 