using System.Text;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Workouts.GetExercisesByWorkout;

public class GetExercisesByWorkoutCommandHandler : BaseTelegramCommandHandler<GetExercisesByWorkoutCommand>
{
    private readonly IWorkoutsApiClient _workoutsApiClient;
    private readonly IUserSessionService _sessionService;

    public GetExercisesByWorkoutCommandHandler(
        ITelegramBotService botService,
        IWorkoutsApiClient workoutsApiClient,
        IUserSessionService sessionService,
        ILogger<GetExercisesByWorkoutCommandHandler>? logger = null)
        : base(botService, logger)
    {
        _workoutsApiClient = workoutsApiClient;
        _sessionService = sessionService;
    }

    protected override async Task<Result> ExecuteAsync(GetExercisesByWorkoutCommand request, CancellationToken cancellationToken)
    {
        var chatId = request.Message.ChatId;

        try
        {
            var token = await _sessionService.GetAuthTokenAsync(chatId, cancellationToken);
            if (token == null)
            {
                return await BotService.SendMessageAsync(chatId, "Вы не авторизованы. Пожалуйста, войдите в систему с помощью /login.", cancellationToken);
            }

            var result = await _workoutsApiClient.GetExercisesByWorkoutIdAsync(request.WorkoutId, token, cancellationToken);

            if (result.IsSuccess)
            {
                if (result.Value != null && result.Value.Any())
                {
                    var sb = new StringBuilder();
                    var workout = await _workoutsApiClient.GetWorkoutByIdAsync(request.WorkoutId, token, cancellationToken);
                    var workoutName = workout.IsSuccess ? workout.Value!.Name : $"ID {request.WorkoutId}";
                    sb.AppendLine($"*Упражнения в тренировке \"{workoutName}\":*");
                
                    foreach (var exercise in result.Value)
                    {
                        var exerciseInfoResult = await _workoutsApiClient.GetExerciseInfoByIdAsync(exercise.ExerciseInfoId, token, cancellationToken);
                        var exerciseInfoName = exerciseInfoResult.IsSuccess ? exerciseInfoResult.Value!.Name : "Неизвестное упражнение";
                    
                        sb.AppendLine();
                        sb.AppendLine($"▶️ *{exerciseInfoName}*");
                        sb.AppendLine($"  ID: `{exercise.Id}`");
                    }
                    return await BotService.SendMessageAsync(chatId, sb.ToString(), cancellationToken: cancellationToken);
                }
                
                return await BotService.SendMessageAsync(chatId, "В этой тренировке пока нет упражнений.", cancellationToken);
            }
            
            return await BotService.SendMessageAsync(chatId, $"❌ Не удалось получить упражнения. Ошибка: {result.Error}", cancellationToken);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to get exercises for workout {WorkoutId} for chat {ChatId}", request.WorkoutId, chatId);
            return await BotService.SendMessageAsync(chatId, "❌ Произошла неожиданная ошибка при получении упражнений.", cancellationToken);
        }
    }
} 