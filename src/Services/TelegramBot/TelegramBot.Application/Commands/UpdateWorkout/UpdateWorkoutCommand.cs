using TelegramBot.Application.Commands.Base;

namespace TelegramBot.Application.Commands.UpdateWorkout;

public class UpdateWorkoutCommand : BaseTelegramCommand
{
    public Guid WorkoutId { get; init; }
    public string? Notes { get; init; }
} 