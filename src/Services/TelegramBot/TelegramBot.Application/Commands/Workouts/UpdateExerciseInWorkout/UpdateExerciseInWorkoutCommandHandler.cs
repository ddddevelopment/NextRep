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

            // TODO: ExerciseUpdateDto в Workouts.Api не содержит поля Notes.
            // Это нужно исправить в API, чтобы можно было обновлять заметки.
            // var updateDto = new ExerciseUpdateDto { Notes = request.Notes };
            
            // Пока что мы не можем ничего обновить, поэтому просто вернем сообщение.
            // В будущем здесь будет вызов _workoutsApiClient.UpdateExerciseAsync
            
            return await BotService.SendMessageAsync(chatId, "⚠️ Функция обновления заметок для упражнения временно недоступна.", cancellationToken);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Failed to update exercise {ExerciseId} in workout {WorkoutId} for chat {ChatId}", request.ExerciseId, request.WorkoutId, chatId);
            return await BotService.SendMessageAsync(chatId, "❌ Произошла неожиданная ошибка при обновлении упражнения.", cancellationToken);
        }
    }
} 