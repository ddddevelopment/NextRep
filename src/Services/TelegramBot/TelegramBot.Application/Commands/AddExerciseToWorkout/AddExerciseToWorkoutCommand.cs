using TelegramBot.Application.Commands.Base;

namespace TelegramBot.Application.Commands.AddExerciseToWorkout;

public class AddExerciseToWorkoutCommand : BaseTelegramCommand
{
    public Guid WorkoutId { get; init; }
    public Guid ExerciseInfoId { get; init; }
    public string? Notes { get; init; }
} 