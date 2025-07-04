using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;

namespace TelegramBot.Application.Commands.Workouts.DeleteExerciseFromWorkout;

public class DeleteExerciseFromWorkoutCommand : BaseTelegramCommand
{
    public Guid WorkoutId { get; }
    public Guid ExerciseId { get; }

    public DeleteExerciseFromWorkoutCommand(TelegramMessage message, Guid workoutId, Guid exerciseId)
    {
        Message = message;
        WorkoutId = workoutId;
        ExerciseId = exerciseId;
    }
} 