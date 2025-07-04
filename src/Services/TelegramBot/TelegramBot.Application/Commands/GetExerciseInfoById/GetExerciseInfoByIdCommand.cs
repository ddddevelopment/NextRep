using TelegramBot.Application.Commands.Base;

namespace TelegramBot.Application.Commands.GetExerciseInfoById;

public class GetExerciseInfoByIdCommand : BaseTelegramCommand
{
    public Guid ExerciseInfoId { get; init; }
} 