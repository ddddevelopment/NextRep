using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Workouts;

public class WorkoutsCommandHandler : BaseTelegramCommandHandler<WorkoutsCommand> {
    private readonly IWorkoutsApiClient _workoutsApiClient;
    private readonly IUserSessionService _userSessionService;

    public WorkoutsCommandHandler(
        ITelegramBotService botService, 
        IWorkoutsApiClient workoutsApiClient,
        IUserSessionService userSessionService,
        ILogger<WorkoutsCommandHandler>? logger = null) 
        : base(botService, logger) {
        _workoutsApiClient = workoutsApiClient;
        _userSessionService = userSessionService;
    }

    protected override async Task<Result> ExecuteAsync(WorkoutsCommand request, CancellationToken cancellationToken) {
        var chatId = request.Message.ChatId;
        
        // Проверяем авторизацию пользователя
        var isAuthenticated = await _userSessionService.IsUserAuthenticatedAsync(chatId, cancellationToken);
        if (!isAuthenticated)
        {
            var notAuthMessage = "🚫 *Для работы с тренировками требуется авторизация*\n\n" +
                                "Пожалуйста, войдите в систему через команду /login или зарегистрируйтесь через /register";
            
            return await BotService.SendMessageAsync(chatId, notAuthMessage, cancellationToken);
        }

        var authToken = await _userSessionService.GetAuthTokenAsync(chatId, cancellationToken);
        if (string.IsNullOrEmpty(authToken))
        {
            var tokenErrorMessage = "❌ *Ошибка авторизации*\n\nПопробуйте войти в систему заново через /login";
            return await BotService.SendMessageAsync(chatId, tokenErrorMessage, cancellationToken);
        }

        // Получаем тренировки пользователя
                    var workoutsResult = await _workoutsApiClient.GetAllWorkoutsAsync(authToken, cancellationToken);
        
        if (!workoutsResult.IsSuccess)
        {
            var errorMessage = "❌ *Ошибка при получении тренировок*\n\n" +
                              $"Причина: {workoutsResult.Error?.Message ?? "Неизвестная ошибка"}";
            
            return await BotService.SendMessageAsync(chatId, errorMessage, cancellationToken);
        }

        var workouts = workoutsResult.Value?.ToList() ?? new List<WorkoutDto>();
        
        string workoutsMessage;
        
        if (!workouts.Any())
        {
            workoutsMessage = "💪 *Мои тренировки*\n\n" +
                             "📈 *Последние тренировки:*\n" +
                             "• Пока тренировок нет\n\n" +
                             "🎯 *Быстрые действия:*\n" +
                             "• Создать новую тренировку\n" +
                             "• Выбрать шаблон тренировки\n" +
                             "• Повторить последнюю\n\n" +
                             "💡 _Начните с создания своей первой тренировки!_";
        }
        else
        {
            var recentWorkouts = workouts
                .OrderByDescending(w => w.StartTime != default(DateTime) ? w.StartTime : w.Date)
                .Take(5)
                .ToList();

            var workoutsText = string.Join("\n", recentWorkouts.Select(w => 
            {
                var workoutDate = w.StartTime != default(DateTime) ? w.StartTime : w.Date;
                var duration = w.EndTime != default(DateTime) && w.StartTime != default(DateTime) 
                    ? (w.EndTime - w.StartTime).TotalMinutes 
                    : w.Duration?.TotalMinutes ?? 0;
                
                return $"• *{w.Name}* - {workoutDate:dd.MM.yyyy}" +
                       (duration > 0 ? $" ({duration:0} мин)" : "") +
                       $" (ID: `{w.Id}`)";
            }));

            var totalWorkouts = workouts.Count;
            var thisWeekWorkouts = workouts.Count(w => 
            {
                var workoutDate = w.StartTime != default(DateTime) ? w.StartTime : w.Date;
                return workoutDate >= DateTime.Now.AddDays(-7);
            });
            var totalMinutes = workouts.Sum(w => 
            {
                if (w.EndTime != default(DateTime) && w.StartTime != default(DateTime))
                    return (w.EndTime - w.StartTime).TotalMinutes;
                return w.Duration?.TotalMinutes ?? 0;
            });

            workoutsMessage = "💪 *Мои тренировки*\n\n" +
                             "📈 *Последние тренировки:*\n" +
                             workoutsText + "\n\n" +
                             "🎯 *Быстрые действия:*\n" +
                             "• Создать новую тренировку\n" +
                             "• Выбрать шаблон тренировки\n" +
                             "• Повторить последнюю\n\n" +
                             "📊 *Статистика за неделю:*\n" +
                             $"• Тренировок: {thisWeekWorkouts}\n" +
                             $"• Время: {totalMinutes:0} мин\n" +
                             $"• Всего тренировок: {totalWorkouts}";
        }

        return await BotService.SendMessageAsync(chatId, workoutsMessage, cancellationToken);
    }
} 