using MediatR;
using TelegramBot.Domain.Models;

namespace TelegramBot.Application.Commands
{
    public record RegisterUserCommand(
        long TelegramId,
        string Email,
        string Password,
        string FirstName,
        string LastName
    ) : IRequest<Result<UserDto>>;
    
    public record LoginUserCommand(
        long TelegramId,
        string Email,
        string Password
    ) : IRequest<Result<AuthTokenDto>>;
    
    public record ProcessTelegramUpdateCommand(
        long TelegramId,
        string? FirstName,
        string? LastName,
        string? Username,
        string MessageText,
        int MessageId
    ) : IRequest<Result<string>>;
    
    public record CreateWorkoutCommand(
        long TelegramId,
        CreateWorkoutDto Workout
    ) : IRequest<Result<WorkoutDto>>;
}