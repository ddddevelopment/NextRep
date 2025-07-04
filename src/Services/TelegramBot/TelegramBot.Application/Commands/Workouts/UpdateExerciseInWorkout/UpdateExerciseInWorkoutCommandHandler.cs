using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.Workouts.UpdateExerciseInWorkout;

public class UpdateExerciseInWorkoutCommandHandler : BaseTelegramCommandHandler<UpdateExerciseInWorkoutCommand>
{
    private readonly IWorkoutsApiClient _workoutsApiClient;
    private readonly IUserSessionService _sessionService;

    public UpdateExerciseInWorkoutCommandHandler(
        ITelegramBotService botService,
        IWorkoutsApiClient workoutsApiClient,
        IUserSessionService sessionService,
        ILogger<UpdateExerciseInWorkoutCommandHandler>? logger = null)
        : base(botService, logger)
    {
        _workoutsApiClient = workoutsApiClient;
        _sessionService = sessionService;
    }

    protected override async Task<Result> ExecuteAsync(UpdateExerciseInWorkoutCommand request, CancellationToken cancellationToken)
    {
        var chatId = request.Message.ChatId;

        try
        {
            var token = await _sessionService.GetAuthTokenAsync(chatId, cancellationToken);
            if (token == null)
            {
                return await BotService.SendMessageAsync(chatId, "Вы не авторизованы. Пожалуйста, войдите в систему с помощью /login.", cancellationToken);
            }

            // Сначала получаем текущее упражнение, чтобы не потерять его данные
            var exerciseResult = await _workoutsApiClient.GetExerciseByIdAsync(request.WorkoutId, request.ExerciseId, token, cancellationToken);
            if (!exerciseResult.IsSuccess)
            {
                return await BotService.SendMessageAsync(chatId, $"❌ Не удалось найти упражнение для обновления. Ошибка: {exerciseResult.Error}", cancellationToken);
            }
            
            var existingExercise = exerciseResult.Value!;
            var updateDto = new ExerciseUpdateDto
            {
                ExerciseInfoId = existingExercise.ExerciseInfoId, // Сохраняем существующий ExerciseInfoId
                Notes = request.Notes // Устанавливаем новые заметки
            };

            var result = await _workoutsApiClient.UpdateExerciseAsync(request.WorkoutId, request.ExerciseId, updateDto, token, cancellationToken);

            if (result.IsSuccess)
            {
                return await BotService.SendMessageAsync(chatId, "✅ Заметки для упражнения успешно обновлены.", cancellationToken);
            }
            
            return await BotService.SendMessageAsync(chatId, $"❌ Не удалось обновить упражнение. Ошибка: {result.Error}", cancellationToken);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to update exercise {ExerciseId} in workout {WorkoutId} for chat {ChatId}", request.ExerciseId, request.WorkoutId, chatId);
            return await BotService.SendMessageAsync(chatId, "❌ Произошла неожиданная ошибка при обновлении упражнения.", cancellationToken);
        }
    }
} 