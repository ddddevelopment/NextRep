using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.UpdateExerciseInfo;

public class UpdateExerciseInfoCommandHandler : BaseTelegramCommandHandler<UpdateExerciseInfoCommand>
{
    private readonly IUserSessionService _sessionService;
    private readonly IWorkoutsApiClient _workoutsApiClient;

    public UpdateExerciseInfoCommandHandler(
        ITelegramBotService telegramBotService,
        IUserSessionService sessionService,
        IWorkoutsApiClient workoutsApiClient,
        ILogger<UpdateExerciseInfoCommandHandler> logger) : base(telegramBotService, logger)
    {
        _sessionService = sessionService;
        _workoutsApiClient = workoutsApiClient;
    }

    protected override async Task<Result> ExecuteAsync(UpdateExerciseInfoCommand request, CancellationToken cancellationToken)
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

        var updateDto = new ExerciseInfoUpdateDto
        {
            Name = request.Name,
            MuscleGroup = request.MuscleGroup,
            Description = request.Description
        };

        var result = await _workoutsApiClient.UpdateExerciseInfoAsync(request.ExerciseInfoId, updateDto, token, cancellationToken);

        if (result.IsSuccess)
        {
            await BotService.SendMessageAsync(chatId, "✅ Упражнение успешно обновлено.", cancellationToken: cancellationToken);
        }
        else
        {
            var errorMessage = result.Error?.Message ?? "Не удалось обновить упражнение.";
            await BotService.SendMessageAsync(chatId, $"❌ Ошибка: {errorMessage}", cancellationToken: cancellationToken);
        }

        return Result.Success();
    }
} 