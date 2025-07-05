using TelegramBot.Application.Commands.Base;

namespace TelegramBot.Application.Commands.GetSetsInExercise;

public class GetSetsInExerciseCommand : BaseTelegramCommand
{
    public Guid WorkoutId { get; init; }
    public Guid ExerciseId { get; init; }
} 