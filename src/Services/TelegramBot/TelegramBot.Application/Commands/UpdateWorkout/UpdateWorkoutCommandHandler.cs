using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.UpdateWorkout;

public class UpdateWorkoutCommandHandler : BaseTelegramCommandHandler<UpdateWorkoutCommand>
{
    private readonly IUserSessionService _sessionService;
    private readonly IWorkoutsApiClient _workoutsApiClient;

    public UpdateWorkoutCommandHandler(
        ITelegramBotService telegramBotService,
        IUserSessionService sessionService,
        IWorkoutsApiClient workoutsApiClient,
        ILogger<UpdateWorkoutCommandHandler> logger) : base(telegramBotService, logger)
    {
        _sessionService = sessionService;
        _workoutsApiClient = workoutsApiClient;
    }

    protected override async Task<Result> ExecuteAsync(UpdateWorkoutCommand request, CancellationToken cancellationToken)
    {
        var chatId = request.Message.ChatId;
        if (!await _sessionService.IsUserAuthenticatedAsync(chatId, cancellationToken))
        {
            await BotService.SendMessageAsync(chatId, "Вы не авторизованы. Пожалуйста, используйте /login.", cancellationToken: cancellationToken);
            return Result.Success();
        }

        var token = await _sessionService.GetAuthTokenAsync(chatId, cancellationToken);
        if (token is null)
        {
            await BotService.SendMessageAsync(chatId, "Ваша сессия истекла. Пожалуйста, используйте /login.", cancellationToken: cancellationToken);
            return Result.Failure("Auth token is null");
        }

        // 1. Получаем текущую тренировку, чтобы сохранить StartTime и EndTime
        var getResult = await _workoutsApiClient.GetWorkoutByIdAsync(request.WorkoutId, token, cancellationToken);
        if (!getResult.IsSuccess)
        {
            await BotService.SendMessageAsync(chatId, "❌ Не удалось найти тренировку для обновления.", cancellationToken: cancellationToken);
            return Result.Failure($"Workout with id {request.WorkoutId} not found.");
        }

        var existingWorkout = getResult.Value;

        // 2. Создаем DTO для обновления
        var updateDto = new WorkoutUpdateDto
        {
            StartTime = existingWorkout.StartTime,
            EndTime = existingWorkout.EndTime,
            Notes = request.Notes
        };

        // 3. Отправляем запрос на обновление
        var updateResult = await _workoutsApiClient.UpdateWorkoutAsync(request.WorkoutId, updateDto, token, cancellationToken);

        if (updateResult.IsSuccess)
        {
            await BotService.SendMessageAsync(chatId, "✅ Заметки к тренировке успешно обновлены.", cancellationToken: cancellationToken);
        }
        else
        {
            var errorMessage = updateResult.Error?.Message ?? "Не удалось обновить тренировку.";
            await BotService.SendMessageAsync(chatId, $"❌ Ошибка: {errorMessage}", cancellationToken: cancellationToken);
        }

        return Result.Success();
    }
} 