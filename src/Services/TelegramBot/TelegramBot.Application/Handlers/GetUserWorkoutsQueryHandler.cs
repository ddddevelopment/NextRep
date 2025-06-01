using TelegramBot.Application.Queries;
using MediatR;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace GymTracker.TelegramBot.Application.Handlers
{
    public class GetUserWorkoutsQueryHandler : IRequestHandler<GetUserWorkoutsQuery, Result<List<WorkoutDto>>>
    {
        private readonly IWorkoutService _workoutService;
        private readonly ITelegramUserService _telegramUserService;

        public GetUserWorkoutsQueryHandler(
            IWorkoutService workoutService,
            ITelegramUserService telegramUserService)
        {
            _workoutService = workoutService;
            _telegramUserService = telegramUserService;
        }

        public async Task<Result<List<WorkoutDto>>> Handle(GetUserWorkoutsQuery request, CancellationToken cancellationToken)
        {
            var telegramUserResult = await _telegramUserService.GetOrCreateTelegramUserAsync(
                request.TelegramId, null, null, null);

            if (!telegramUserResult.IsSuccess || !telegramUserResult.Value!.UserId.HasValue)
                return Result<List<WorkoutDto>>.Failure("Пользователь не авторизован");

            return await _workoutService.GetUserWorkoutsAsync(
                telegramUserResult.Value.UserId.Value, 
                request.Page, 
                request.PageSize);
        }
    }
}