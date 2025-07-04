using System.Text;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Workouts.GetExerciseInWorkoutById;

public class GetExerciseInWorkoutByIdCommandHandler : BaseTelegramCommandHandler<GetExerciseInWorkoutByIdCommand>
{
    private readonly IWorkoutsApiClient _workoutsApiClient;
    private readonly IUserSessionService _sessionService;

    public GetExerciseInWorkoutByIdCommandHandler(
        ITelegramBotService botService,
        IWorkoutsApiClient workoutsApiClient,
        IUserSessionService sessionService,
        ILogger<GetExerciseInWorkoutByIdCommandHandler>? logger = null)
        : base(botService, logger)
    {
        _workoutsApiClient = workoutsApiClient;
        _sessionService = sessionService;
    }

    protected override async Task<Result> ExecuteAsync(GetExerciseInWorkoutByIdCommand request, CancellationToken cancellationToken)
    {
        var chatId = request.Message.ChatId;

        try
        {
            var token = await _sessionService.GetAuthTokenAsync(chatId, cancellationToken);
            if (token == null)
            {
                return await BotService.SendMessageAsync(chatId, "Вы не авторизованы. Пожалуйста, войдите в систему с помощью /login.", cancellationToken);
            }

            var result = await _workoutsApiClient.GetExerciseByIdAsync(request.WorkoutId, request.ExerciseId, token, cancellationToken);

            if (result.IsSuccess && result.Value != null)
            {
                var exercise = result.Value;
                var exerciseInfoResult = await _workoutsApiClient.GetExerciseInfoByIdAsync(exercise.ExerciseInfoId, token, cancellationToken);
                var exerciseInfoName = exerciseInfoResult.IsSuccess ? exerciseInfoResult.Value!.Name : "Неизвестное упражнение";
                
                var sb = new StringBuilder();
                sb.AppendLine($"▶️ *{exerciseInfoName}*");
                sb.AppendLine($"  ID упражнения: `{exercise.Id}`");
                if (!string.IsNullOrWhiteSpace(exercise.Notes))
                {
                    sb.AppendLine($"  Заметки: _{exercise.Notes}_");
                }
                
                // TODO: Добавить вывод подходов (сетов), когда будет реализована их выборка.
                if (exercise.Sets != null && exercise.Sets.Any())
                {
                    sb.AppendLine($"  *Подходы:*");
                    foreach (var set in exercise.Sets)
                    {
                        sb.AppendLine($"    - Сет {set.Order}: {set.Reps} раз по {set.Weight} кг");
                    }
                }
                
                return await BotService.SendMessageAsync(chatId, sb.ToString(), cancellationToken: cancellationToken);
            }
            
            return await BotService.SendMessageAsync(chatId, $"❌ Не удалось получить упражнение. Ошибка: {result.Error}", cancellationToken);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to get exercise {ExerciseId} in workout {WorkoutId} for chat {ChatId}", request.ExerciseId, request.WorkoutId, chatId);
            return await BotService.SendMessageAsync(chatId, "❌ Произошла неожиданная ошибка при получении упражнения.", cancellationToken);
        }
    }
} 