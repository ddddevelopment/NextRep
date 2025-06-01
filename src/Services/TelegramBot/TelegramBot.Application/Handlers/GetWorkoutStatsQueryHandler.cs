using MediatR;
using TelegramBot.Application.Queries;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace GymTracker.TelegramBot.Application.Handlers
{
    public class GetWorkoutStatsQueryHandler : IRequestHandler<GetWorkoutStatsQuery, Result<WorkoutStatsDto>>
    {
        private readonly IWorkoutService _workoutService;
        private readonly ITelegramUserService _telegramUserService;

        public GetWorkoutStatsQueryHandler(
            IWorkoutService workoutService,
            ITelegramUserService telegramUserService)
        {
            _workoutService = workoutService;
            _telegramUserService = telegramUserService;
        }

        public async Task<Result<WorkoutStatsDto>> Handle(GetWorkoutStatsQuery request, CancellationToken cancellationToken)
        {
            var telegramUserResult = await _telegramUserService.GetOrCreateTelegramUserAsync(
                request.TelegramId, null, null, null);

            if (!telegramUserResult.IsSuccess || !telegramUserResult.Value!.UserId.HasValue)
                return Result<WorkoutStatsDto>.Failure("Пользователь не авторизован");

            return await _workoutService.GetWorkoutStatsAsync(telegramUserResult.Value.UserId.Value);
        }
    }
}
