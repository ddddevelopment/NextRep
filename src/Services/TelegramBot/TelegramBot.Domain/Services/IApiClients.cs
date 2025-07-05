using TelegramBot.Domain.Models;

namespace TelegramBot.Domain.Services;

public interface IUsersApiClient
{
    Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UserDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<Guid>> CreateAsync(UserCreateDto user, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(UserUpdateDto user, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IAuthApiClient
{
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto login, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto register, CancellationToken cancellationToken = default);
}

public interface IWorkoutsApiClient
{
    // Workouts
    Task<Result<IEnumerable<WorkoutDto>>> GetAllWorkoutsAsync(string token, CancellationToken cancellationToken = default);
    Task<Result<WorkoutDto>> GetWorkoutByIdAsync(Guid id, string token, CancellationToken cancellationToken = default);
    Task<Result> CreateWorkoutAsync(WorkoutCreateDto workout, string token, CancellationToken cancellationToken = default);
    Task<Result> UpdateWorkoutAsync(Guid id, WorkoutUpdateDto workout, string token, CancellationToken cancellationToken = default);
    Task<Result> DeleteWorkoutAsync(Guid id, string token, CancellationToken cancellationToken = default);
    
    // Exercises
    Task<Result<IEnumerable<ExerciseDto>>> GetExercisesByWorkoutIdAsync(Guid workoutId, string token, CancellationToken cancellationToken = default);
    Task<Result<ExerciseDto>> GetExerciseByIdAsync(Guid workoutId, Guid exerciseId, string token, CancellationToken cancellationToken = default);
    Task<Result> CreateExerciseAsync(Guid workoutId, ExerciseCreateDto exercise, string token, CancellationToken cancellationToken = default);
    Task<Result> UpdateExerciseAsync(Guid workoutId, Guid exerciseId, ExerciseUpdateDto exercise, string token, CancellationToken cancellationToken = default);
    Task<Result> DeleteExerciseAsync(Guid workoutId, Guid exerciseId, string token, CancellationToken cancellationToken = default);
    
    // Sets
    Task<Result<IEnumerable<SetDto>>> GetSetsByExerciseIdAsync(Guid workoutId, Guid exerciseId, string token, CancellationToken cancellationToken = default);
    Task<Result<SetDto>> GetSetByIdAsync(Guid workoutId, Guid exerciseId, Guid setId, string token, CancellationToken cancellationToken = default);
    Task<Result> CreateSetAsync(Guid workoutId, Guid exerciseId, SetCreateRequestDto set, string token, CancellationToken cancellationToken = default);
    Task<Result> UpdateSetAsync(Guid workoutId, Guid exerciseId, Guid setId, SetUpdateRequestDto set, string token, CancellationToken cancellationToken = default);
    Task<Result> DeleteSetAsync(Guid workoutId, Guid exerciseId, Guid setId, string token, CancellationToken cancellationToken = default);
    
    // ExerciseInfo
    Task<Result<IEnumerable<ExerciseInfoDto>>> GetExerciseInfosAsync(string token, CancellationToken cancellationToken = default);
    Task<Result<ExerciseInfoDto>> GetExerciseInfoByIdAsync(Guid id, string token, CancellationToken cancellationToken = default);
    Task<Result> CreateExerciseInfoAsync(ExerciseInfoCreateDto exerciseInfo, string token, CancellationToken cancellationToken = default);
    Task<Result> UpdateExerciseInfoAsync(Guid id, ExerciseInfoUpdateDto exerciseInfo, string token, CancellationToken cancellationToken = default);
    Task<Result> DeleteExerciseInfoAsync(Guid id, string token, CancellationToken cancellationToken = default);
}
