using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;
using Microsoft.Extensions.Logging;

namespace TelegramBot.Application.Commands.AddSetToExercise;

public class AddSetToExerciseCommandHandler : BaseTelegramCommandHandler<AddSetToExerciseCommand>
{
    private readonly IUserSessionService _sessionService;
    private readonly IWorkoutsApiClient _workoutsApiClient;

    public AddSetToExerciseCommandHandler(
        ITelegramBotService telegramBotService,
        IUserSessionService sessionService,
        IWorkoutsApiClient workoutsApiClient,
        ILogger<AddSetToExerciseCommandHandler> logger) : base(telegramBotService, logger)
    {
        _sessionService = sessionService;
        _workoutsApiClient = workoutsApiClient;
    }

    protected override async Task<Result> ExecuteAsync(AddSetToExerciseCommand request, CancellationToken cancellationToken)
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

        // Get current sets to determine the order for the new set
        var existingSetsResult = await _workoutsApiClient.GetSetsByExerciseIdAsync(request.WorkoutId, request.ExerciseId, token, cancellationToken);
        if (!existingSetsResult.IsSuccess)
        {
            var errorMessage = existingSetsResult.Error?.Message ?? "Неизвестная ошибка";
            await BotService.SendMessageAsync(chatId, $"❌ Ошибка при получении существующих подходов: {errorMessage}", cancellationToken: cancellationToken);
            return Result.Failure(errorMessage);
        }

        var newSetOrder = existingSetsResult.Value.Count() + 1;

        var setToCreate = new SetCreateDto
        {
            Weight = request.Weight,
            Reps = request.Reps,
            Order = newSetOrder
        };

        var createResult = await _workoutsApiClient.CreateSetAsync(request.WorkoutId, request.ExerciseId, setToCreate, token, cancellationToken);

        if (createResult.IsSuccess)
        {
            await BotService.SendMessageAsync(chatId, "✅ Подход успешно добавлен!", cancellationToken: cancellationToken);
        }
        else
        {
            var errorMessage = createResult.Error?.Message ?? "Не удалось добавить подход.";
            await BotService.SendMessageAsync(chatId, $"❌ Ошибка: {errorMessage}", cancellationToken: cancellationToken);
        }

        return Result.Success();
    }
} 