using Telegram.Bot.Types;
using TelegramBot.Application.Commands.Base;
using TelegramBot.Domain.Models;

namespace TelegramBot.Application.Commands.Workouts.GetExercisesByWorkout;

public class GetExercisesByWorkoutCommand : BaseTelegramCommand
{
    public Guid WorkoutId { get; }

    public GetExercisesByWorkoutCommand(TelegramMessage message, Guid workoutId)
    {
        Message = message;
        WorkoutId = workoutId;
    }
} 