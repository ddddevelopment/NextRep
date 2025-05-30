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

        public async Task<Result> Create(Set set)
        {
            _logger?.LogDebug("Attempting to create set: {@Set}", set);

            if (set == null)
            {
                _logger?.LogWarning("Set is null");
                return Result.Invalid("Set must not be null");
            }

            var exerciseExistsResult = await _exercisesService.GetById(set.ExerciseId);
            if (!exerciseExistsResult.IsSuccess)
            {
                _logger?.LogWarning("Exercise with ID {ExerciseId} not found when trying to add set", set.ExerciseId);
                return Result.NotFound($"Exercise with ID {set.ExerciseId} not found");
            }

            var result = await _repository.Add(set);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Set created successfully: {@Set}", set);
            }

            return result;
        }

        public async Task<Result<Set>> GetByIdInExercise(Guid exerciseId, Guid id)
        {
            _logger?.LogDebug("Fetching set with ID: {SetId}", id);

            var exerciseExistsResult = await _exercisesService.GetById(exerciseId);
            if (!exerciseExistsResult.IsSuccess)
            {
                _logger?.LogWarning("Exercise with ID {ExerciseId} not found when trying to get set", exerciseId);
                return Result<Set>.NotFound($"Exercise with ID {exerciseId} not found");
            }

            var result = await _repository.GetByIdInExercise(exerciseId, id);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Set retrieved successfully: {@Set}", result.Value);
            }

            return result;
        }

        public async Task<Result<IEnumerable<Set>>> GetAllByExerciseId(Guid exerciseId)
        {
            _logger?.LogDebug("Fetching sets for exercise ID: {ExerciseId}", exerciseId);

            var exerciseExistsResult = await _exercisesService.GetById(exerciseId);
            if (!exerciseExistsResult.IsSuccess)
            {
                _logger?.LogWarning("Exercise with ID {ExerciseId} not found when trying to get sets", exerciseId);
                return Result<IEnumerable<Set>>.NotFound($"Exercise with ID {exerciseId} not found");
            }

            var result = await _repository.GetAllByExerciseId(exerciseId);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Successfully retrieved sets for exercise ID: {ExerciseId}", exerciseId);
            }

            return result;
        }

        public async Task<Result<Set>> Update(Set set)
        {
            _logger?.LogDebug("Attempting to update set: {@Set}", set);

            if (set == null)
            {
                _logger?.LogWarning("Set is null");
                return Result<Set>.Invalid("Set must not be null");
            }

            var exerciseExistsResult = await _exercisesService.GetById(set.ExerciseId);
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

        public async Task<Result> DeleteFromExercise(Guid exerciseId, Guid id)
        {
            _logger?.LogDebug("Attempting to delete set with ID: {SetId}", id);

            var exerciseExistsResult = await _exercisesService.GetById(exerciseId);
            if (!exerciseExistsResult.IsSuccess)
            {
                _logger?.LogWarning("Exercise ID {ExerciseId} not found when deleting set ID {SetId}", exerciseId, id);
                return Result.NotFound($"Exercise with ID {exerciseId} not found");
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
