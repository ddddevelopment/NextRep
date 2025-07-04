using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.CreateExerciseInfo;

public class CreateExerciseInfoCommandHandler : BaseTelegramCommandHandler<CreateExerciseInfoCommand>
{
    private readonly IUserSessionService _sessionService;
    private readonly IWorkoutsApiClient _workoutsApiClient;

    public CreateExerciseInfoCommandHandler(
        ITelegramBotService telegramBotService,
        IUserSessionService sessionService,
        IWorkoutsApiClient workoutsApiClient,
        ILogger<CreateExerciseInfoCommandHandler> logger) : base(telegramBotService, logger)
    {
        _sessionService = sessionService;
        _workoutsApiClient = workoutsApiClient;
    }

    protected override async Task<Result> ExecuteAsync(CreateExerciseInfoCommand request, CancellationToken cancellationToken)
    {
        var chatId = request.Message.ChatId;
        if (!await _sessionService.IsUserAuthenticatedAsync(chatId, cancellationToken))
        {
            await BotService.SendMessageAsync(
                chatId,
                "Вы не авторизованы. Пожалуйста, используйте /login.",
                cancellationToken: cancellationToken);
            return Result.Success();
        }

        var token = await _sessionService.GetAuthTokenAsync(chatId, cancellationToken);
        if (token is null)
        {
            await BotService.SendMessageAsync(
                chatId,
                "Ваша сессия истекла. Пожалуйста, используйте /login.",
                cancellationToken: cancellationToken);
            return Result.Failure("Auth token is null");
        }

        var createDto = new ExerciseInfoCreateDto
        {
            Name = request.Name,
            MuscleGroup = request.MuscleGroup,
            Description = request.Description
        };

        var result = await _workoutsApiClient.CreateExerciseInfoAsync(createDto, token, cancellationToken);

        if (result.IsSuccess)
        {
            await BotService.SendMessageAsync(
                chatId,
                $"✅ Упражнение '{request.Name}' успешно создано!",
                cancellationToken: cancellationToken);
        }
        else
        {
            var errorMessage = result.Error?.Message ?? "Произошла неизвестная ошибка.";
            await BotService.SendMessageAsync(
                chatId,
                $"❌ Не удалось создать упражнение. Ошибка: {errorMessage}",
                cancellationToken: cancellationToken);
        }

        return Result.Success();
    }
} 