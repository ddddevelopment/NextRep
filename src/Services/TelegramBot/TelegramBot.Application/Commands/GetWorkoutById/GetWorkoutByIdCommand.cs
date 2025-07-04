using TelegramBot.Application.Commands.Base;

namespace TelegramBot.Application.Commands.GetWorkoutById;

public class GetWorkoutByIdCommand : BaseTelegramCommand
{
    public Guid WorkoutId { get; init; }
} 