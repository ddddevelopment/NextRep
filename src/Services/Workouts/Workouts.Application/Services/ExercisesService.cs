using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Workouts.Application.Services
{
    public class ExercisesService : IExercisesService
    {
        private readonly IExercisesRepository _repository;
        private readonly IWorkoutsService _workoutsService;
        private readonly IExerciseInfosService _exerciseInfosService;
        private readonly ILogger<ExercisesService>? _logger;

        public ExercisesService(IExercisesRepository repository, IWorkoutsService workoutsService, ExerciseInfosService exerciseInfosService, ILogger<ExercisesService>? logger = null)
        {
            _repository = repository;
            _workoutsService = workoutsService;
            _exerciseInfosService = exerciseInfosService;
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

            Result<Workout> workoutExistsResult = await _workoutsService.GetById(exercise.WorkoutId);
            if (workoutExistsResult.IsSuccess == false)
            {
                _logger?.LogWarning("Workout with ID {WorkoutId} not found when trying to add exercise", exercise.WorkoutId);
                return Result.NotFound($"Workout with ID {exercise.WorkoutId} not found");
            }

            Result<ExerciseInfo> exerciseInfoExistsResult = await _exerciseInfosService.GetById(exercise.ExerciseInfoId);
            if (exerciseInfoExistsResult.IsSuccess == false)
            {
                _logger?.LogWarning("ExerciseInfo with ID {ExerciseInfoId} not found when trying to add exercise", exercise.ExerciseInfoId);
                return Result.NotFound($"ExerciseInfo with ID {exercise.ExerciseInfoId} not found");
            }

            Result result = await _repository.Add(exercise);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Exercise created successfully: {@Exercise}", exercise);
            }

            return result;
        }

        public async Task<Result<Exercise>> GetByIdInWorkout(Guid workoutId, Guid id)
        {
            _logger?.LogDebug("Fetching exercise with ID: {ExerciseId}", id);

            Result<Workout> workoutExistsResult = await _workoutsService.GetById(workoutId);
            if (workoutExistsResult.IsSuccess == false)
            {
                _logger?.LogWarning("Workout with ID {WorkoutId} not found when trying to get exercise", workoutId);
                return Result<Exercise>.NotFound($"Workout with ID {workoutId} not found");
            }

            Result<Exercise> result = await _repository.GetById(id);

            if (result.IsSuccess)
            {
                if (result.Value!.WorkoutId != workoutId)
                {
                    _logger?.LogWarning("Exercise ID {ExerciseId} found, but it does not belong to workout ID {WorkoutId}", id, workoutId);
                    return Result<Exercise>.NotFound($"Exercise with ID {id} not found in workout {workoutId}");
                }

                _logger?.LogInformation("Exercise retrieved successfully: {@Exercise}", result.Value);
            }

            return result;
        }

        public async Task<Result<IEnumerable<Exercise>>> GetAllByWorkoutId(Guid workoutId)
        {
            _logger?.LogDebug("Fetching exercises for workout ID: {WorkoutId}", workoutId);

            Result<Workout> workoutExistsResult = await _workoutsService.GetById(workoutId);
            if (workoutExistsResult.IsSuccess == false)
            {
                _logger?.LogWarning("Workout with ID {WorkoutId} not found when trying to get exercises", workoutId);
                return Result<IEnumerable<Exercise>>.NotFound($"Workout with ID {workoutId} not found");
            }

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

            Result<Workout> workoutExistsResult = await _workoutsService.GetById(exercise.WorkoutId);
            if (workoutExistsResult.IsSuccess == false)
            {
                _logger?.LogWarning("Workout ID {WorkoutId} not found when updating exercise ID {ExerciseId}", exercise.WorkoutId, exercise.Id);
                return Result<Exercise>.NotFound($"Workout with ID {exercise.WorkoutId} not found");
            }

            Result<Exercise> currentExerciseResult = await _repository.GetById(exercise.Id);
            if (currentExerciseResult.IsSuccess == false)
            {
                _logger?.LogWarning("Exercise ID {ExerciseId} not found for update in workout ID {WorkoutId}. Result: {ErrorMessage}", exercise.Id, exercise.WorkoutId, currentExerciseResult.Error?.Message);
                return currentExerciseResult;
            }
            else if (currentExerciseResult.Value!.WorkoutId != exercise.WorkoutId)
            {
                _logger?.LogWarning("Attempt to update exercise ID {ExerciseId} which does not belong to workout ID {WorkoutId}", exercise.Id, exercise.WorkoutId);
                return Result<Exercise>.NotFound($"Exercise {exercise.Id} does not belong to workout {exercise.WorkoutId}");
            }

            Result<Exercise> result = await _repository.Update(exercise);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Exercise updated successfully: {@Exercise}", result.Value);
            }

            return result;
        }

        public async Task<Result> Delete(Guid workoutId, Guid id)
        {
            _logger?.LogDebug("Attempting to delete exercise with ID: {ExerciseId}", id);

            Result<Workout> workoutExistsResult = await _workoutsService.GetById(workoutId);
            if (workoutExistsResult.IsSuccess == false)
            {
                _logger?.LogWarning("Workout ID {WorkoutId} not found when deleting exercise ID {ExerciseId}", workoutId, id);
                return Result.NotFound($"Workout with ID {workoutId} not found");
            }

            Result<Exercise> currentExerciseResult = await _repository.GetById(id);
            if (currentExerciseResult.IsSuccess == false)
            {
                _logger?.LogWarning("Exercise ID {ExerciseId} not found for deletion in workout ID {WorkoutId}. Result: {ErrorMessage}", id, workoutId, currentExerciseResult.Error?.Message);
                return Result.NotFound($"Exercise with ID {id} not found");
            }
            else if (currentExerciseResult.Value!.WorkoutId != workoutId)
            {
                _logger?.LogWarning("Attempt to delete exercise ID {ExerciseId} which does not belong to workout ID {WorkoutId}.", id, workoutId);
                return Result.NotFound($"Exercise {id} does not belong to workout {workoutId}");
            }

            Result result = await _repository.Delete(id);

            if (result.IsSuccess)
            {
                _logger?.LogInformation("Exercise deleted successfully with ID: {ExerciseId}", id);
            }

            return result;
        }
    }
}