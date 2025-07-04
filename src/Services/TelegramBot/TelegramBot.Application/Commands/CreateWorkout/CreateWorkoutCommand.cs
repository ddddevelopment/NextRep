using TelegramBot.Application.Commands.Base;

namespace TelegramBot.Application.Commands.CreateWorkout;

public class CreateWorkoutCommand : BaseTelegramCommand
{
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string? Notes { get; init; }
} 