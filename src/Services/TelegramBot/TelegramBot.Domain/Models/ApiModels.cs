namespace TelegramBot.Domain.Models;

// Users API DTOs - исправлено в соответствии с реальным API
public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}

public class UserCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
}

public class UserUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
}

// Auth API DTOs - исправлено в соответствии с реальным API
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string? AccessToken { get; set; }
    public int? ExpiresIn { get; set; }
    public string? ErrorMessage { get; set; }
}

// Workouts API DTOs - базовые модели из реального API
public class WorkoutDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? Duration { get; set; }
    public Guid UserId { get; set; }
    public List<ExerciseDto> Exercises { get; set; } = new();
    
    // Дополнительные поля для совместимости с реальным API
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Notes { get; set; }
}

public class WorkoutCreateDto
{
    public required string Name { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Notes { get; set; }
}

public class WorkoutUpdateDto
{
    public required string Name { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Notes { get; set; }
}

public class ExerciseDto
{
    public Guid Id { get; set; }
    public Guid WorkoutId { get; set; }
    public Guid ExerciseInfoId { get; set; }
    public List<SetDto> Sets { get; set; } = new();
    public string? Notes { get; set; } = string.Empty;
}

public class ExerciseCreateDto
{
    public Guid ExerciseInfoId { get; set; }
    public string? Notes { get; set; }
}

public class ExerciseUpdateDto
{
    public Guid ExerciseInfoId { get; set; }
    public string? Notes { get; set; }
}

public class ExerciseInfoDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string MuscleGroup { get; set; } = string.Empty;
}

public class ExerciseInfoCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string MuscleGroup { get; set; } = string.Empty;
}

public class ExerciseInfoUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string MuscleGroup { get; set; } = string.Empty;
}

public class SetDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public int? Reps { get; set; }
    public decimal? Weight { get; set; }
    public TimeSpan? Duration { get; set; }
    public decimal? Distance { get; set; }
    public string? Notes { get; set; }
    public Guid ExerciseId { get; set; }
}

public class SetCreateDto
{
    public int Order { get; set; }
    public int? Reps { get; set; }
    public decimal? Weight { get; set; }
    public TimeSpan? Duration { get; set; }
    public decimal? Distance { get; set; }
    public string? Notes { get; set; }
}

public class SetUpdateDto
{
    public int Order { get; set; }
    public int? Reps { get; set; }
    public decimal? Weight { get; set; }
    public TimeSpan? Duration { get; set; }
    public decimal? Distance { get; set; }
    public string? Notes { get; set; }
} 