using MediatR;
using TelegramBot.Domain.Models;

namespace TelegramBot.Application.Queries;

public record GetUserWorkoutsQuery(
    long TelegramId,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<List<WorkoutDto>>>;

public record GetWorkoutStatsQuery(
    long TelegramId
) : IRequest<Result<WorkoutStatsDto>>;

public record GetTelegramUserQuery(
    long TelegramId
) : IRequest<Result<TelegramUser>>;
