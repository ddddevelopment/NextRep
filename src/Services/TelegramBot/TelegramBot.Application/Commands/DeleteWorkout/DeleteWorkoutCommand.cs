using TelegramBot.Application.Commands.Base;

namespace TelegramBot.Application.Commands.DeleteWorkout;
 
public class DeleteWorkoutCommand : BaseTelegramCommand
{
    public Guid WorkoutId { get; init; }
} 