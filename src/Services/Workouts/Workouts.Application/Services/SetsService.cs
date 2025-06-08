using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Workouts.Application.Services
{
    public class SetsService : ISetsService
    {
        private readonly ISetsRepository _repository;
        private readonly IExercisesService _exercisesService;
        private readonly ILogger<SetsService>? _logger;

        public SetsService(ISetsRepository repository, IExercisesService exercisesService, ILogger<SetsService>? logger = null)
        {
            _repository = repository;
            _exercisesService = exercisesService;
            _logger = logger;
        }

        public async Task<Result> Create(Guid workoutId, Set set)
        {
            _logger?.LogDebug("Attempting to create set in workout {WorkoutId}: {@Set}", workoutId, set);

            if (set == null)
            {
                _logger?.LogWarning("Set is null (workout {WorkoutId})", workoutId);
                return Result.Invalid("Set must not be null");
            }

            var exerciseExistsResult = await _exercisesService.GetByIdInWorkout(workoutId, set.ExerciseId);
            if (!exerciseExistsResult.IsSuccess)
            {
                _logger?.LogWarning("Exercise with ID {ExerciseId} not found in workout {WorkoutId} when trying to add set", set.ExerciseId, workoutId);
                return Result.NotFound($"Exercise with ID {set.ExerciseId} not found in workout {workoutId}");
            }

            var result = await _repository.Add(set);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Set created successfully in workout {WorkoutId}: {@Set}", workoutId, set);
            }

            return result;
        }

        public async Task<Result<Set>> GetByIdInExercise(Guid workoutId, Guid exerciseId, Guid id)
        {
            _logger?.LogDebug("Fetching set with ID: {SetId}", id);

            var exerciseResult = await _exercisesService.GetByIdInWorkout(workoutId, exerciseId);
            if (!exerciseResult.IsSuccess)
            {
                _logger?.LogWarning("Exercise with ID {ExerciseId} not found in workout {WorkoutId} when trying to get set", exerciseId, workoutId);
                return Result<Set>.NotFound($"Exercise {exerciseId} not found in workout {workoutId}");
            }

            var result = await _repository.GetByIdInExercise(exerciseId, id);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Set retrieved successfully: {@Set}", result.Value);
            }

            return result;
        }

        public async Task<Result<IEnumerable<Set>>> GetAllByExerciseId(Guid workoutId, Guid exerciseId)
        {
            _logger?.LogDebug("Fetching sets for exercise ID: {ExerciseId}", exerciseId);

            var exerciseResult = await _exercisesService.GetByIdInWorkout(workoutId, exerciseId);
            if (!exerciseResult.IsSuccess)
            {
                _logger?.LogWarning("Exercise with ID {ExerciseId} not found in workout {WorkoutId} when trying to get sets", exerciseId, workoutId);
                return Result<IEnumerable<Set>>.NotFound($"Exercise {exerciseId} not found in workout {workoutId}");
            }

            var result = await _repository.GetAllByExerciseId(exerciseId);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Successfully retrieved sets for exercise ID: {ExerciseId}", exerciseId);
            }

            return result;
        }

        public async Task<Result<Set>> Update(Guid workoutId, Set set)
        {
            _logger?.LogDebug("Attempting to update set: {@Set}", set);

            if (set == null)
            {
                _logger?.LogWarning("Set is null");
                return Result<Set>.Invalid("Set must not be null");
            }

            var exerciseExistsResult = await _exercisesService.GetByIdInWorkout(workoutId, set.ExerciseId);
            if (!exerciseExistsResult.IsSuccess)
            {
                _logger?.LogWarning("Exercise ID {ExerciseId} not found when updating set ID {SetId}", set.ExerciseId, set.Id);
                return Result<Set>.NotFound($"Exercise with ID {set.ExerciseId} not found");
            }

            var result = await _repository.Update(set);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Set updated successfully: {@Set}", result.Value);
            }

            return result;
        }

        public async Task<Result> DeleteFromExercise(Guid workoutId, Guid exerciseId, Guid id)
        {
            _logger?.LogDebug("Attempting to delete set with ID: {SetId}", id);

            var exerciseResult = await _exercisesService.GetByIdInWorkout(workoutId, exerciseId);
            if (!exerciseResult.IsSuccess)
            {
                _logger?.LogWarning("Exercise ID {ExerciseId} not found in workout {WorkoutId} when deleting set ID {SetId}", exerciseId, workoutId, id);
                return Result.NotFound($"Exercise {exerciseId} not found in workout {workoutId}");
            }

            var result = await _repository.DeleteFromExercise(exerciseId, id);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Set deleted successfully with ID: {SetId}", id);
            }

            return result;
        }
    }
}
