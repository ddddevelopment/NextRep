using MediatR;
using TelegramBot.Application.Commands;
using TelegramBot.Domain.Models;
using TelegramBot.Domain.Services;

namespace GymTracker.TelegramBot.Application.Handlers
{
    public class CreateWorkoutCommandHandler : IRequestHandler<CreateWorkoutCommand, Result<WorkoutDto>>
    {
        private readonly IWorkoutService _workoutService;
        private readonly ITelegramUserService _telegramUserService;

        public CreateWorkoutCommandHandler(
            IWorkoutService workoutService,
            ITelegramUserService telegramUserService)
        {
            _workoutService = workoutService;
            _telegramUserService = telegramUserService;
        }

        public async Task<Result<WorkoutDto>> Handle(CreateWorkoutCommand request, CancellationToken cancellationToken)
        {
            var telegramUserResult = await _telegramUserService.GetOrCreateTelegramUserAsync(
                request.TelegramId, null, null, null);

            if (!telegramUserResult.IsSuccess || !telegramUserResult.Value!.UserId.HasValue)
                return Result<WorkoutDto>.Failure("Пользователь не авторизован");

            return await _workoutService.CreateWorkoutAsync(
                telegramUserResult.Value.UserId.Value, 
                request.Workout);
        }
    }
}