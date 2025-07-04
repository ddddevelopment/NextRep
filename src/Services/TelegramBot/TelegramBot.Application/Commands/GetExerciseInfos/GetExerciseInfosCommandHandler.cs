using System.Text;
using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.GetExerciseInfos;

public class GetExerciseInfosCommandHandler : BaseTelegramCommandHandler<GetExerciseInfosCommand>
{
    private readonly IUserSessionService _sessionService;
    private readonly IWorkoutsApiClient _workoutsApiClient;

    public GetExerciseInfosCommandHandler(
        ITelegramBotService telegramBotService,
        IUserSessionService sessionService,
        IWorkoutsApiClient workoutsApiClient,
        ILogger<GetExerciseInfosCommandHandler> logger) : base(telegramBotService, logger)
    {
        _sessionService = sessionService;
        _workoutsApiClient = workoutsApiClient;
    }

    protected override async Task<Result> ExecuteAsync(GetExerciseInfosCommand request, CancellationToken cancellationToken)
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

        var result = await _workoutsApiClient.GetExerciseInfosAsync(token, cancellationToken);

        if (!result.IsSuccess || !result.Value.Any())
        {
            var errorMessage = result.Error?.Message ?? "У вас пока нет созданных упражнений.";
            await BotService.SendMessageAsync(chatId, $"ℹ️ {errorMessage}", cancellationToken: cancellationToken);
            return Result.Success();
        }

        var sb = new StringBuilder();
        sb.AppendLine("📋 *Список ваших упражнений:*\n");

        foreach (var exerciseInfo in result.Value)
        {
            sb.AppendLine($"🔹 *Название:* {exerciseInfo.Name}");
            sb.AppendLine($"   *Группа мышц:* `{exerciseInfo.MuscleGroup}`");
            sb.AppendLine($"   *ID:* `{exerciseInfo.Id}`\n");
        }

        sb.AppendLine("\nИспользуйте ID для команд /update_exercise_info и /delete_exercise_info.");

        await BotService.SendMessageAsync(chatId, sb.ToString(), cancellationToken: cancellationToken);
        return Result.Success();
    }
} 