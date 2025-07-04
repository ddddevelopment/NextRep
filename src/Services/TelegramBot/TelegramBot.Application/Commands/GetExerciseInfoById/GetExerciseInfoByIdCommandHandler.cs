using System.Text;
using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.GetExerciseInfoById;

public class GetExerciseInfoByIdCommandHandler : BaseTelegramCommandHandler<GetExerciseInfoByIdCommand>
{
    private readonly IUserSessionService _sessionService;
    private readonly IWorkoutsApiClient _workoutsApiClient;

    public GetExerciseInfoByIdCommandHandler(
        ITelegramBotService telegramBotService,
        IUserSessionService sessionService,
        IWorkoutsApiClient workoutsApiClient,
        ILogger<GetExerciseInfoByIdCommandHandler> logger) : base(telegramBotService, logger)
    {
        _sessionService = sessionService;
        _workoutsApiClient = workoutsApiClient;
    }

    protected override async Task<Result> ExecuteAsync(GetExerciseInfoByIdCommand request, CancellationToken cancellationToken)
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

        var result = await _workoutsApiClient.GetExerciseInfoByIdAsync(request.ExerciseInfoId, token, cancellationToken);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Error?.Message ?? "Не удалось найти упражнение с таким ID.";
            await BotService.SendMessageAsync(chatId, $"❌ {errorMessage}", cancellationToken: cancellationToken);
            return Result.Success();
        }

        var sb = new StringBuilder();
        var exerciseInfo = result.Value;
        
        sb.AppendLine($"*Название:* {exerciseInfo.Name}");
        sb.AppendLine($"*Группа мышц:* {exerciseInfo.MuscleGroup}");
        if(!string.IsNullOrEmpty(exerciseInfo.Description))
        {
            sb.AppendLine($"*Описание:* {exerciseInfo.Description}");
        }
        sb.AppendLine($"*ID:* `{exerciseInfo.Id}`");

        await BotService.SendMessageAsync(chatId, sb.ToString(), cancellationToken: cancellationToken);
        return Result.Success();
    }
} 