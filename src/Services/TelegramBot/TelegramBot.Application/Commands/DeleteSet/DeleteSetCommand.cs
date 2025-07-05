using TelegramBot.Application.Commands.Base;
using System;

namespace TelegramBot.Application.Commands.DeleteSet;

public class DeleteSetCommand : BaseTelegramCommand
{
    public Guid WorkoutId { get; init; }
    public Guid ExerciseId { get; init; }
    public Guid SetId { get; init; }
} 