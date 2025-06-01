using TelegramBot.Domain.Models;

namespace TelegramBot.Domain.Services;

public interface IUserService
{
    Task<Result<UserDto>> RegisterAsync(RegisterUserDto registerDto);
    Task<Result<AuthTokenDto>> LoginAsync(LoginDto loginDto);
    Task<Result<UserDto>> GetUserByIdAsync(Guid userId);
}

public interface IWorkoutService
{
    Task<Result<WorkoutDto>> CreateWorkoutAsync(Guid userId, CreateWorkoutDto workout);
    Task<Result<List<WorkoutDto>>> GetUserWorkoutsAsync(Guid userId, int page, int pageSize);
    Task<Result<WorkoutStatsDto>> GetWorkoutStatsAsync(Guid userId);
}

public interface ITelegramUserService
{
    Task<Result<TelegramUser>> GetOrCreateTelegramUserAsync(long telegramId, string? firstName, string? lastName, string? username);
    Task<Result<TelegramUser>> UpdateUserStateAsync(long telegramId, UserState state, string? temporaryData = null);
    Task<Result<TelegramUser>> LinkUserAccountAsync(long telegramId, Guid userId);
}

