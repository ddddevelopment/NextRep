using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Workouts.Application.Services
{
    public class ExercisesService : IExercisesService
    {
        private readonly IExercisesRepository _repository;
        private readonly ILogger<ExercisesService>? _logger;

        public ExercisesService(IExercisesRepository repository, ILogger<ExercisesService>? logger = null)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result> Create(Exercise exercise)
        {
            _logger?.LogDebug("Attempting to create exercise: {@Exercise}", exercise);

            if (exercise == null)
            {
                _logger?.LogWarning("Exercise is null");
                return Result.Invalid("Exercise must not be null");
            }

            // Дополнительная логика валидации может быть здесь
            // Например, проверка WorkoutId, ExerciseInfoId и т.д.

            Result result = await _repository.Add(exercise);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Exercise created successfully: {@Exercise}", exercise);
            }

            return result;
        }

        public async Task<Result<Exercise>> GetById(Guid id)
        {
            _logger?.LogDebug("Fetching exercise with ID: {ExerciseId}", id);

            Result<Exercise> result = await _repository.GetById(id);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Exercise retrieved successfully: {@Exercise}", result.Value);
            }

            return result;
        }

        public async Task<Result<IEnumerable<Exercise>>> GetByWorkoutId(Guid workoutId)
        {
            _logger?.LogDebug("Fetching exercises for workout ID: {WorkoutId}", workoutId);

            Result<IEnumerable<Exercise>> result = await _repository.GetByWorkoutId(workoutId);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Successfully retrieved exercises for workout ID: {WorkoutId}", workoutId);
            }
            
            return result;
        }

        public async Task<Result<Exercise>> Update(Exercise exercise)
        {
            _logger?.LogDebug("Attempting to update exercise: {@Exercise}", exercise);

            if (exercise == null)
            {
                _logger?.LogWarning("Exercise is null");
                return Result<Exercise>.Invalid("Exercise must not be null");
            }

            Result<Exercise> result = await _repository.Update(exercise);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Exercise updated successfully: {@Exercise}", result.Value);
            }

            return result;
        }

        public async Task<Result> Delete(Guid id)
        {
            _logger?.LogDebug("Attempting to delete exercise with ID: {ExerciseId}", id);

            Result result = await _repository.Delete(id);

            if (result.IsSuccess) 
            {
                _logger?.LogInformation("Exercise deleted successfully with ID: {ExerciseId}", id);
            }
            
            return result;
        }
    }
} 