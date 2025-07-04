using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;

namespace TelegramBot.Application.Commands.Workouts.GetExerciseInWorkoutById;

public class GetExerciseInWorkoutByIdCommand : BaseTelegramCommand
{
    public Guid WorkoutId { get; }
    public Guid ExerciseId { get; }

    public GetExerciseInWorkoutByIdCommand(TelegramMessage message, Guid workoutId, Guid exerciseId)
    {
        Message = message;
        WorkoutId = workoutId;
        ExerciseId = exerciseId;
    }
} 