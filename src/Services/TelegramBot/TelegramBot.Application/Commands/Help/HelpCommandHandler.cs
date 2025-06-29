using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Help;

public class HelpCommandHandler : BaseTelegramCommandHandler<HelpCommand> {
    public HelpCommandHandler(ITelegramBotService botService, ILogger<HelpCommandHandler>? logger = null) 
        : base(botService, logger) {
    }

    protected override async Task<Result> ExecuteAsync(HelpCommand request, CancellationToken cancellationToken) {
        var helpMessage = "🤖 *Справка по командам NextRep Bot*\n\n" +
                         "📋 */menu* - Главное меню с быстрыми действиями\n" +
                         "💪 */workouts* - Просмотр и управление тренировками\n" +
                         "👤 */profile* - Настройки профиля\n" +
                         "📊 */stats* - Статистика тренировок\n" +
                         "❓ */help* - Эта справка\n\n" +
                         "*Возможности бота:*\n" +
                         "• Создание и отслеживание тренировок\n" +
                         "• Добавление упражнений и подходов\n" +
                         "• Просмотр статистики прогресса\n" +
                         "• Управление профилем\n\n" +
                         "Для начала используйте /menu";

        return await BotService.SendMessageAsync(request.Message.ChatId, helpMessage, cancellationToken);
    }
} 