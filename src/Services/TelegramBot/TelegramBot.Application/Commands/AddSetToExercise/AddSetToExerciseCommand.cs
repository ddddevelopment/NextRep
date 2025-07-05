using TelegramBot.Application.Commands.Base;

namespace TelegramBot.Application.Commands.AddSetToExercise;

public class AddSetToExerciseCommand : BaseTelegramCommand
{
    public Guid WorkoutId { get; init; }
    public Guid ExerciseId { get; init; }
    public decimal? Weight { get; init; }
    public int? Reps { get; init; }
    public string? Notes { get; init; }
    // We can add other properties like Duration, Distance, Notes later
} 