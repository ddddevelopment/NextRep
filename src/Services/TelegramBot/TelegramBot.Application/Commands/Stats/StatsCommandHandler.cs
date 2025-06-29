using MediatR;
using Microsoft.Extensions.Logging;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Stats;

public class StatsCommandHandler : IRequestHandler<StatsCommand, Result> {
    private readonly ITelegramBotService _botService;
    private readonly ILogger<StatsCommandHandler>? _logger;

    public StatsCommandHandler(ITelegramBotService botService, ILogger<StatsCommandHandler>? logger = null) {
        _botService = botService;
        _logger = logger;
    }

    public async Task<Result> Handle(StatsCommand request, CancellationToken cancellationToken) {
        _logger?.LogInformation("Handling /stats command for user {UserId}", request.Message.From.Id);
        
        var statsMessage = "📊 *Статистика тренировок*\n\n" +
                          "📅 *За все время:*\n" +
                          "• Тренировок: 0\n" +
                          "• Упражнений: 0\n" +
                          "• Подходов: 0\n" +
                          "• Время: 0 ч 0 мин\n\n" +
                          "📈 *За этот месяц:*\n" +
                          "• Тренировок: 0\n" +
                          "• Среднее время: 0 мин\n" +
                          "• Самая длинная: 0 мин\n\n" +
                          "🔥 *Активность:*\n" +
                          "• Текущая серия: 0 дней\n" +
                          "• Лучшая серия: 0 дней\n" +
                          "• Последняя тренировка: никогда\n\n" +
                          "🏆 *Достижения:*\n" +
                          "• Пока нет достижений\n\n" +
                          "_Начните тренироваться, чтобы увидеть прогресс! /workouts_";

        return await _botService.SendMessageAsync(request.Message.ChatId, statsMessage, cancellationToken);
    }
} 