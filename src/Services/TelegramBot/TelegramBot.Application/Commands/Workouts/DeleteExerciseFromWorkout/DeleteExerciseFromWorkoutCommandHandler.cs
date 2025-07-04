using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Workouts.DeleteExerciseFromWorkout;

public class DeleteExerciseFromWorkoutCommandHandler : BaseTelegramCommandHandler<DeleteExerciseFromWorkoutCommand>
{
    private readonly IWorkoutsApiClient _workoutsApiClient;
    private readonly IUserSessionService _sessionService;

    public DeleteExerciseFromWorkoutCommandHandler(
        ITelegramBotService botService,
        IWorkoutsApiClient workoutsApiClient,
        IUserSessionService sessionService,
        ILogger<DeleteExerciseFromWorkoutCommandHandler>? logger = null)
        : base(botService, logger)
    {
        _workoutsApiClient = workoutsApiClient;
        _sessionService = sessionService;
    }

    protected override async Task<Result> ExecuteAsync(DeleteExerciseFromWorkoutCommand request, CancellationToken cancellationToken)
    {
        var chatId = request.Message.ChatId;

        try
        {
            var token = await _sessionService.GetAuthTokenAsync(chatId, cancellationToken);
            if (token == null)
            {
                return await BotService.SendMessageAsync(chatId, "Вы не авторизованы. Пожалуйста, войдите в систему с помощью /login.", cancellationToken);
            }

            var result = await _workoutsApiClient.DeleteExerciseAsync(request.WorkoutId, request.ExerciseId, token, cancellationToken);

            if (result.IsSuccess)
            {
                return await BotService.SendMessageAsync(chatId, "✅ Упражнение успешно удалено из тренировки.", cancellationToken);
            }
            
            return await BotService.SendMessageAsync(chatId, $"❌ Не удалось удалить упражнение. Ошибка: {result.Error}", cancellationToken);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to delete exercise {ExerciseId} from workout {WorkoutId} for chat {ChatId}", request.ExerciseId, request.WorkoutId, chatId);
            return await BotService.SendMessageAsync(chatId, "❌ Произошла неожиданная ошибка при удалении упражнения.", cancellationToken);
        }
    }
} 