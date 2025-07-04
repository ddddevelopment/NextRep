using TelegramBot.Application.Commands.Base;

namespace TelegramBot.Application.Commands.DeleteExerciseInfo;

public class DeleteExerciseInfoCommand : BaseTelegramCommand
{
    public Guid ExerciseInfoId { get; init; }
} 