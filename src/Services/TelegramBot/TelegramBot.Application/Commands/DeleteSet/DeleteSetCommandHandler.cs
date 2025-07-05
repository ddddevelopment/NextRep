using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace TelegramBot.Application.Commands.DeleteSet;

public class DeleteSetCommandHandler : BaseTelegramCommandHandler<DeleteSetCommand>
{
    private readonly IUserSessionService _sessionService;
    private readonly IWorkoutsApiClient _workoutsApiClient;

    public DeleteSetCommandHandler(
        ITelegramBotService telegramBotService,
        IUserSessionService sessionService,
        IWorkoutsApiClient workoutsApiClient,
        ILogger<DeleteSetCommandHandler> logger) : base(telegramBotService, logger)
    {
        _sessionService = sessionService;
        _workoutsApiClient = workoutsApiClient;
    }

    protected override async Task<Result> ExecuteAsync(DeleteSetCommand request, CancellationToken cancellationToken)
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

        var deleteResult = await _workoutsApiClient.DeleteSetAsync(request.WorkoutId, request.ExerciseId, request.SetId, token, cancellationToken);

        if (deleteResult.IsSuccess)
        {
            await BotService.SendMessageAsync(chatId, $"✅ Подход ID: {request.SetId} успешно удален!", cancellationToken: cancellationToken);
        }
        else
        {
            var errorMessage = deleteResult.Error?.Message ?? "Не удалось удалить подход.";
            await BotService.SendMessageAsync(chatId, $"❌ Ошибка: {errorMessage}", cancellationToken: cancellationToken);
        }

        return Result.Success();
    }
} 