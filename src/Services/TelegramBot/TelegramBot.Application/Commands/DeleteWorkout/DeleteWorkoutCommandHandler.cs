using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.DeleteWorkout;

public class DeleteWorkoutCommandHandler : BaseTelegramCommandHandler<DeleteWorkoutCommand>
{
    private readonly IUserSessionService _sessionService;
    private readonly IWorkoutsApiClient _workoutsApiClient;

    public DeleteWorkoutCommandHandler(
        ITelegramBotService telegramBotService,
        IUserSessionService sessionService,
        IWorkoutsApiClient workoutsApiClient,
        ILogger<DeleteWorkoutCommandHandler> logger) : base(telegramBotService, logger)
    {
        _sessionService = sessionService;
        _workoutsApiClient = workoutsApiClient;
    }

    protected override async Task<Result> ExecuteAsync(DeleteWorkoutCommand request, CancellationToken cancellationToken)
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

        var result = await _workoutsApiClient.DeleteWorkoutAsync(request.WorkoutId, token, cancellationToken);

        if (result.IsSuccess)
        {
            await BotService.SendMessageAsync(chatId, $"✅ Тренировка с ID `{request.WorkoutId}` успешно удалена.", cancellationToken: cancellationToken);
        }
        else
        {
            var errorMessage = result.Error?.Message ?? "Не удалось удалить тренировку.";
            await BotService.SendMessageAsync(chatId, $"❌ Ошибка: {errorMessage}", cancellationToken: cancellationToken);
        }

        return Result.Success();
    }
} 