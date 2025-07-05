using System.Text;
using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.GetSetsInExercise;

public class GetSetsInExerciseCommandHandler : BaseTelegramCommandHandler<GetSetsInExerciseCommand>
{
    private readonly IUserSessionService _sessionService;
    private readonly IWorkoutsApiClient _workoutsApiClient;

    public GetSetsInExerciseCommandHandler(
        ITelegramBotService telegramBotService,
        IUserSessionService sessionService,
        IWorkoutsApiClient workoutsApiClient,
        ILogger<GetSetsInExerciseCommandHandler> logger) : base(telegramBotService, logger)
    {
        _sessionService = sessionService;
        _workoutsApiClient = workoutsApiClient;
    }

    protected override async Task<Result> ExecuteAsync(GetSetsInExerciseCommand request, CancellationToken cancellationToken)
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

        var result = await _workoutsApiClient.GetSetsByExerciseIdAsync(request.WorkoutId, request.ExerciseId, token, cancellationToken);

        if (result.IsSuccess)
        {
            var sets = result.Value;
            var sb = new StringBuilder();

            if (sets.Any())
            {
                sb.AppendLine($"*Подходы для упражнения (ID: `{request.ExerciseId}`)*");
                int i = 1;
                foreach (var set in sets)
                {
                    var note = string.IsNullOrEmpty(set.Notes) ? "" : $"\\n    *Заметка:* {set.Notes}";
                    sb.AppendLine($"  `{i++}`. *Вес:* {set.Weight} кг, *Повторения:* {set.Reps} (ID: `{set.Id}`){note}");
                }
            }
            else
            {
                sb.AppendLine("Подходов для этого упражнения пока нет.");
            }
            
            await BotService.SendMessageAsync(chatId, sb.ToString(), cancellationToken: cancellationToken);
        }
        else
        {
            var errorMessage = result.Error?.Message ?? "Не удалось получить информацию о подходах.";
            await BotService.SendMessageAsync(chatId, $"❌ Ошибка: {errorMessage}", cancellationToken: cancellationToken);
        }

        return Result.Success();
    }
} 