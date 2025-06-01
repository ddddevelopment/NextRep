namespace TelegramBot.Domain.Models;


public record WorkoutDto(
    Guid Id,
    string Name,
    DateTime Date,
    List<ExerciseDto> Exercises,
    Guid UserId
);

public record ExerciseDto(
    Guid Id,
    string Name,
    List<SetDto> Sets
);

public record SetDto(
    int Reps,
    decimal Weight,
    int Order
);

public record CreateWorkoutDto(
    string Name,
    DateTime Date,
    List<CreateExerciseDto> Exercises
);

public record CreateExerciseDto(
    string Name,
    List<CreateSetDto> Sets
);

public record CreateSetDto(
    int Reps,
    decimal Weight
);

public record WorkoutStatsDto(
    int TotalWorkouts,
    Dictionary<string, decimal> MaxWeights,
    Dictionary<string, int> TotalSets,
    List<ProgressDto> Progress
);

public record ProgressDto(
    string ExerciseName,
    DateTime Date,
    decimal MaxWeight
);
