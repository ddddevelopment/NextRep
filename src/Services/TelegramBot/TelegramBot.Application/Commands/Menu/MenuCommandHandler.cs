using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Menu;

public class MenuCommandHandler : BaseTelegramCommandHandler<MenuCommand> {
    public MenuCommandHandler(ITelegramBotService botService, ILogger<MenuCommandHandler>? logger = null) 
        : base(botService, logger) {
    }

    protected override async Task<Result> ExecuteAsync(MenuCommand request, CancellationToken cancellationToken) {
        var menuMessage = "📋 *Главное меню NextRep*\n\n" +
                         "Выберите действие:\n\n" +
                         "🏋️‍♂️ Тренировки:\n" +
                         "• /workouts - Мои тренировки\n" +
                         "• Создать новую тренировку\n" +
                         "• Продолжить последнюю\n\n" +
                         "📊 Анализ:\n" +
                         "• /stats - Статистика прогресса\n" +
                         "• История тренировок\n" +
                         "• Достижения\n\n" +
                         "⚙️ Настройки:\n" +
                         "• /profile - Мой профиль\n" +
                         "• Уведомления\n" +
                         "• Цели\n\n" +
                         "❓ /help - Помощь";

        return await BotService.SendMessageAsync(request.Message.ChatId, menuMessage, cancellationToken);
    }
} 