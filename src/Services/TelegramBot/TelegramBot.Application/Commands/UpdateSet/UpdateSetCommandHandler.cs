using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace TelegramBot.Application.Commands.UpdateSet;

public class UpdateSetCommandHandler : BaseTelegramCommandHandler<UpdateSetCommand>
{
    private readonly IUserSessionService _sessionService;
    private readonly IWorkoutsApiClient _workoutsApiClient;

    public UpdateSetCommandHandler(
        ITelegramBotService telegramBotService,
        IUserSessionService sessionService,
        IWorkoutsApiClient workoutsApiClient,
        ILogger<UpdateSetCommandHandler> logger) : base(telegramBotService, logger)
    {
        _sessionService = sessionService;
        _workoutsApiClient = workoutsApiClient;
    }

    protected override async Task<Result> ExecuteAsync(UpdateSetCommand request, CancellationToken cancellationToken)
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

        var setResult = await _workoutsApiClient.GetSetByIdAsync(request.WorkoutId, request.ExerciseId, request.SetId, token, cancellationToken);

        if (!setResult.IsSuccess)
        {
            await BotService.SendMessageAsync(chatId, $"❌ Ошибка при получении подхода: {setResult.Error?.Message}", cancellationToken: cancellationToken);
            return Result.Failure(setResult.Error?.Message ?? "Unknown error");
        }

        var setToUpdate = setResult.Value;

        var updateDto = new SetUpdateRequestDto
        {
            Weight = (int)(request.NewWeight ?? setToUpdate.Weight ?? 0),
            Reps = request.NewReps ?? setToUpdate.Reps ?? 0,
            Notes = request.Notes ?? setToUpdate.Notes
        };
        
        var updateResult = await _workoutsApiClient.UpdateSetAsync(request.WorkoutId, request.ExerciseId, request.SetId, updateDto, token, cancellationToken);

        if (updateResult.IsSuccess)
        {
            await BotService.SendMessageAsync(chatId, $"✅ Подход ID: {request.SetId} успешно обновлен!", cancellationToken: cancellationToken);
        }
        else
        {
            var errorMessage = updateResult.Error?.Message ?? "Не удалось обновить подход.";
            await BotService.SendMessageAsync(chatId, $"❌ Ошибка: {errorMessage}", cancellationToken: cancellationToken);
        }

        return Result.Success();
    }
} 