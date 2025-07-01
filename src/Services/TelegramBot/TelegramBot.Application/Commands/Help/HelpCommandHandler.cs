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
                         "🔐 *Команды авторизации:*\n" +
                         "• */login* `email password` - Вход в систему\n" +
                         "• */register* `email password firstName lastName telephone` - Регистрация\n" +
                         "• */logout* - Выход из системы\n\n" +
                         "📱 *Основные команды:*\n" +
                         "• */start* - Главная информация\n" +
                         "• */menu* - Главное меню с быстрыми действиями\n" +
                         "• */help* - Эта справка\n\n" +
                         "💪 *Тренировки (требуется авторизация):*\n" +
                         "• */workouts* - Просмотр и управление тренировками\n" +
                         "• */profile* - Информация о профиле\n" +
                         "• */stats* - Статистика тренировок\n\n" +
                         "*Возможности бота:*\n" +
                         "• Полная интеграция с NextRep API\n" +
                         "• Создание и отслеживание тренировок\n" +
                         "• Добавление упражнений и подходов\n" +
                         "• Просмотр статистики прогресса\n" +
                         "• Управление профилем пользователя\n\n" +
                         "🚀 Для начала зарегистрируйтесь или войдите в систему!";

        return await BotService.SendMessageAsync(request.Message.ChatId, helpMessage, cancellationToken);
    }
} 