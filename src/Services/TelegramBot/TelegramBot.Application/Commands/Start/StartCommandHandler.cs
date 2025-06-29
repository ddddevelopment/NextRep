using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Start;

public class StartCommandHandler : BaseTelegramCommandHandler<StartCommand> {
    public StartCommandHandler(ITelegramBotService botService, ILogger<StartCommandHandler>? logger = null) 
        : base(botService, logger) {
    }

    protected override async Task<Result> ExecuteAsync(StartCommand request, CancellationToken cancellationToken) {
        var welcomeMessage = $"Добро пожаловать в NextRep, {request.Message.From.FirstName}! 🏋️‍♂️\n\n" +
                           "Это ваш персональный фитнес-помощник для отслеживания тренировок.\n\n" +
                           "Доступные команды:\n" +
                           "📋 /menu - Главное меню\n" +
                           "❓ /help - Помощь\n" +
                           "💪 /workouts - Мои тренировки\n" +
                           "👤 /profile - Мой профиль\n" +
                           "📊 /stats - Статистика\n\n" +
                           "Начните с команды /menu для быстрого доступа к функциям!";

        return await BotService.SendMessageAsync(request.Message.ChatId, welcomeMessage, cancellationToken);
    }
} 