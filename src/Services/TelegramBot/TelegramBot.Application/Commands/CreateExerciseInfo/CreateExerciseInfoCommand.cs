using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;

namespace TelegramBot.Application.Commands.CreateExerciseInfo;

public class CreateExerciseInfoCommand : BaseTelegramCommand
{
    public string Name { get; init; } = string.Empty;
    public string MuscleGroup { get; init; } = string.Empty;
    public string? Description { get; init; }
} 