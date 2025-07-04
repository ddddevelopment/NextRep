using System.Text;
using Microsoft.Extensions.Logging;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace TelegramBot.Application.Commands.GetWorkoutById;

public class GetWorkoutByIdCommandHandler : BaseTelegramCommandHandler<GetWorkoutByIdCommand>
{
    private readonly IUserSessionService _sessionService;
    private readonly IWorkoutsApiClient _workoutsApiClient;

    public GetWorkoutByIdCommandHandler(
        ITelegramBotService telegramBotService,
        IUserSessionService sessionService,
        IWorkoutsApiClient workoutsApiClient,
        ILogger<GetWorkoutByIdCommandHandler> logger) : base(telegramBotService, logger)
    {
        _sessionService = sessionService;
        _workoutsApiClient = workoutsApiClient;
    }

    protected override async Task<Result> ExecuteAsync(GetWorkoutByIdCommand request, CancellationToken cancellationToken)
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

        var result = await _workoutsApiClient.GetWorkoutByIdAsync(request.WorkoutId, token, cancellationToken);

        if (result.IsSuccess)
        {
            var workout = result.Value;
            var sb = new StringBuilder();
            sb.AppendLine($"*Workout Details (ID: `{workout.Id}`)*");
            sb.AppendLine($"*Name:* {workout.Name}");
            sb.AppendLine($"*Start:* {workout.StartTime:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"*End:* {workout.EndTime:yyyy-MM-dd HH:mm:ss}");
            if (!string.IsNullOrEmpty(workout.Notes))
            {
                sb.AppendLine($"*Notes:* {workout.Notes}");
            }

            if (workout.Exercises.Any())
            {
                sb.AppendLine("\n*Exercises:*");
                foreach (var exercise in workout.Exercises.OrderBy(e => e.Order))
                {
                    sb.AppendLine($"  - *{exercise.Name}* (ID: `{exercise.Id}`)");
                }
            }
            else
            {
                sb.AppendLine("\nNo exercises found for this workout.");
            }
            
            await BotService.SendMessageAsync(chatId, sb.ToString(), cancellationToken: cancellationToken);
        }
        else
        {
            var errorMessage = result.Error?.Message ?? "Не удалось получить информацию о тренировке.";
            await BotService.SendMessageAsync(chatId, $"❌ Ошибка: {errorMessage}", cancellationToken: cancellationToken);
        }

        return Result.Success();
    }
} 