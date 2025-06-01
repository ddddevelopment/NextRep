namespace TelegramBot.Domain.Models;

public enum UserState
{
    Initial,
    WaitingForRegistration,
    WaitingForLogin,
    Authenticated,
    AddingWorkout,
    ViewingStats,
    WaitingForWorkoutName,
    WaitingForExerciseData
}