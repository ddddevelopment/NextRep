using TelegramBot.Application.Commands.Base;
using System;

namespace TelegramBot.Application.Commands.UpdateSet;

public class UpdateSetCommand : BaseTelegramCommand
{
    public Guid WorkoutId { get; init; }
    public Guid ExerciseId { get; init; }
    public Guid SetId { get; init; }
    public decimal? NewWeight { get; init; }
    public int? NewReps { get; init; }
} 