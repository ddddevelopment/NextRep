using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Workouts;

public class WorkoutsCommandHandler : BaseTelegramCommandHandler<WorkoutsCommand> {
    public WorkoutsCommandHandler(ITelegramBotService botService, ILogger<WorkoutsCommandHandler>? logger = null) 
        : base(botService, logger) {
    }

    protected override async Task<Result> ExecuteAsync(WorkoutsCommand request, CancellationToken cancellationToken) {
        var workoutsMessage = "💪 *Мои тренировки*\n\n" +
                             "📈 *Последние тренировки:*\n" +
                             "• Пока тренировок нет\n\n" +
                             "🎯 *Быстрые действия:*\n" +
                             "• Создать новую тренировку\n" +
                             "• Выбрать шаблон тренировки\n" +
                             "• Повторить последнюю\n\n" +
                             "📊 *Статистика за неделю:*\n" +
                             "• Тренировок: 0\n" +
                             "• Время: 0 мин\n" +
                             "• Упражнений: 0\n\n" +
                             "_В будущих версиях здесь будет отображаться реальная информация о ваших тренировках_";

        return await BotService.SendMessageAsync(request.Message.ChatId, workoutsMessage, cancellationToken);
    }
} 