using TelegramBot.Application.Commands.Base;

namespace TelegramBot.Application.Commands.UpdateExerciseInfo;

public class UpdateExerciseInfoCommand : BaseTelegramCommand
{
    public Guid ExerciseInfoId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string MuscleGroup { get; init; } = string.Empty;
    public string? Description { get; init; }
} 