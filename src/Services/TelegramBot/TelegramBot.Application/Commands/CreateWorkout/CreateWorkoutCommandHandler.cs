using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.CreateWorkout;

public class CreateWorkoutCommandHandler : BaseTelegramCommandHandler<CreateWorkoutCommand>
{
    private readonly IUserSessionService _sessionService;
    private readonly IWorkoutsApiClient _workoutsApiClient;

    public CreateWorkoutCommandHandler(
        ITelegramBotService telegramBotService,
        IUserSessionService sessionService,
        IWorkoutsApiClient workoutsApiClient,
        ILogger<CreateWorkoutCommandHandler> logger) : base(telegramBotService, logger)
    {
        _sessionService = sessionService;
        _workoutsApiClient = workoutsApiClient;
    }

    protected override async Task<Result> ExecuteAsync(CreateWorkoutCommand request, CancellationToken cancellationToken)
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
        
        var createDto = new WorkoutCreateDto
        {
            Name = request.Name,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Notes = request.Notes
        };

        var result = await _workoutsApiClient.CreateWorkoutAsync(createDto, token, cancellationToken);

        if (result.IsSuccess)
        {
            // Поскольку API не возвращает ID, мы не можем его показать.
            await BotService.SendMessageAsync(chatId, $"✅ Тренировка успешно создана.", cancellationToken: cancellationToken);
        }
        else
        {
            var errorMessage = result.Error?.Message ?? "Не удалось создать тренировку.";
            await BotService.SendMessageAsync(chatId, $"❌ Ошибка: {errorMessage}", cancellationToken: cancellationToken);
        }

        return Result.Success();
    }
} 