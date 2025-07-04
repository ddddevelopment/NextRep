using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;

namespace TelegramBot.Application.Commands.Workouts.UpdateExerciseInWorkout;

public class UpdateExerciseInWorkoutCommand : BaseTelegramCommand
{
    public Guid WorkoutId { get; }
    public Guid ExerciseId { get; }
    public string? Notes { get; }

    public UpdateExerciseInWorkoutCommand(TelegramMessage message, Guid workoutId, Guid exerciseId, string? notes)
    {
        Message = message;
        WorkoutId = workoutId;
        ExerciseId = exerciseId;
        Notes = notes;
    }
} 