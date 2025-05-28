using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Workouts.DAL.EF.Entities;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;

namespace Workouts.DAL.EF.Repositories
{
    public class ExercisesEFRepository : IExercisesRepository
    {
        private readonly WorkoutsDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ExercisesEFRepository>? _logger;

        public ExercisesEFRepository(WorkoutsDbContext context, IMapper mapper, ILogger<ExercisesEFRepository>? logger = null)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> Add(Exercise exercise)
        {
            _logger?.LogDebug("Adding exercise to database: {@Exercise}", exercise);
            try
            {
                ExerciseEntity entity = _mapper.Map<ExerciseEntity>(exercise);
                _context.Exercises.Add(entity);
                await _context.SaveChangesAsync();
                _logger?.LogDebug("Exercise added to database successfully: {@Exercise}", exercise);
                return Result.Success();
            }
            catch (DbUpdateException exception)
            {
                _logger?.LogError(exception, "Database error occurred while adding exercise: {@Exercise}", exercise);
                return Result.Failure($"Failed to add exercise: {exception.Message}");
            }
        }

        public async Task<Result<Exercise>> GetById(Guid id)
        {
            _logger?.LogDebug("Fetching from database exercise with ID: {ExerciseId}", id);
            ExerciseEntity? found = await _context.Exercises.FindAsync(id);
            if (found == null)
            {
                _logger?.LogWarning("Exercise with ID: {ExerciseId} not found in database", id);
                return Result<Exercise>.NotFound($"Exercise with ID: {id} not found");
            }
            _logger?.LogDebug("Successfully fetched exercise from database: {@Exercise}", found);
            return Result<Exercise>.Success(_mapper.Map<Exercise>(found));
        }

        public async Task<Result<IEnumerable<Exercise>>> GetByWorkoutId(Guid workoutId)
        {
            _logger?.LogDebug("Fetching exercises for workout ID: {WorkoutId} from database", workoutId);
            try
            {
                IEnumerable<ExerciseEntity> found = await _context.Exercises
                                                              .Where(e => e.workout_id == workoutId)
                                                              .ToListAsync();

                IEnumerable<Exercise> exercises = _mapper.Map<IEnumerable<Exercise>>(found);
                _logger?.LogDebug("Successfully fetched exercises for workout ID: {WorkoutId}", workoutId);
                return Result<IEnumerable<Exercise>>.Success(exercises);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error fetching exercises for workout ID: {WorkoutId}", workoutId);
                return Result<IEnumerable<Exercise>>.Failure($"Failed to fetch exercises: {ex.Message}");
            }
        }

        public async Task<Result<Exercise>> Update(Exercise exercise)
        {
            _logger?.LogDebug("Updating exercise in database: {@Exercise}", exercise);
            ExerciseEntity? entity = await _context.Exercises.FindAsync(exercise.Id);
            if (entity == null)
            {
                _logger?.LogWarning("Exercise with ID: {ExerciseId} not found for update in database", exercise.Id);
                return Result<Exercise>.NotFound($"Exercise with ID: {exercise.Id} not found");
            }
            _mapper.Map(exercise, entity);
            try
            {
                _context.Exercises.Update(entity);
                await _context.SaveChangesAsync();
                Exercise updated = _mapper.Map<Exercise>(entity);
                _logger?.LogDebug("Exercise updated successfully in database: {@Exercise}", updated);
                return Result<Exercise>.Success(updated);
            }
            catch (DbUpdateException exception)
            {
                _logger?.LogError(exception, "Database error occurred while updating exercise: {@Exercise}", exercise);
                return Result<Exercise>.Failure($"Failed to update exercise: {exception.Message}");
            }
        }

        public async Task<Result> Delete(Guid id)
        {
            _logger?.LogDebug("Removing exercise with ID: {ExerciseId} from database", id);
            var entity = await _context.Exercises.FindAsync(id);
            if (entity == null)
            {
                _logger?.LogWarning("Exercise with ID: {ExerciseId} not found for removal in database", id);
                return Result.NotFound($"Exercise with ID: {id} not found");
            }
            try
            {
                _context.Exercises.Remove(entity);
                await _context.SaveChangesAsync();
                _logger?.LogDebug("Exercise removed successfully from database with ID: {ExerciseId}", id);
                return Result.Success();
            }
            catch (DbUpdateException exception)
            {
                _logger?.LogError(exception, "Database error occurred while removing exercise with ID: {ExerciseId}", id);
                return Result.Failure($"Failed to remove exercise: {exception.Message}");
            }
        }
    }
}