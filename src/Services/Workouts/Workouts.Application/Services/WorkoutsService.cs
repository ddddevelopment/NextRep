using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;
using Microsoft.Extensions.Logging;


namespace Workouts.Application.Services
{
    public class WorkoutsService : IWorkoutsService
    {
        private readonly IWorkoutsRepository _repository;
        private readonly ILogger<WorkoutsService>? _logger;

        public WorkoutsService(IWorkoutsRepository repository, ILogger<WorkoutsService>? logger = null)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result> Create(Workout workout)
        {
            _logger?.LogDebug("Attempting to create workout: {@Workout}", workout);

            if (workout == null)
            {
                _logger?.LogWarning("Workout is null");
                return Result.Invalid("Workout must not be null");
            }

            Result result = await _repository.Add(workout);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Workout created successfully: {@Workout}", workout);
            }

            return result;
        }

        public async Task<Result<Workout>> GetById(Guid id)
        {
            _logger?.LogDebug("Fetching workout with ID: {WorkoutId}", id);

            Result<Workout> result = await _repository.GetById(id);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Workout retrieved successfully: {@Workout}", result.Value);
            }

            return result;
        }

        public async Task<Result<IEnumerable<Workout>>> GetAllByUserId(Guid userId)
        {
            _logger?.LogDebug("Fetching all workouts for user {UserId}", userId);

            Result<IEnumerable<Workout>> result = await _repository.GetAllByUserId(userId);

            if (result.IsSuccess) {
                _logger?.LogInformation("Successfully retrieved all workouts for user {UserId}", userId);
            }
            
            return result;
        }

        public async Task<Result<Workout>> Update(Workout workout)
        {
            _logger?.LogDebug("Attempting to update workout: {@Workout}", workout);

            if (workout == null)
            {
                _logger?.LogWarning("Workout is null");
                return Result<Workout>.Invalid("Workout must not be null");
            }

            Result<Workout> result = await _repository.Update(workout);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Workout updated successfully: {@Workout}", result.Value);
            }

            return result;
        }

        public async Task<Result> Delete(Guid id)
        {
            _logger?.LogDebug("Attempting to delete workout with ID: {WorkoutId}", id);

            Result result = await _repository.Delete(id);

            if (result.IsSuccess) {
                _logger?.LogInformation("Workout deleted successfully with ID: {WorkoutId}", id);
            }
            
            return result;
        }
    }
}