using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Workouts.DAL.EF.Entities;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;

namespace Workouts.DAL.EF.Repositories
{
    public class SetsEFRepository : ISetsRepository
    {
        private readonly WorkoutsDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<SetsEFRepository>? _logger;

        public SetsEFRepository(WorkoutsDbContext context, IMapper mapper, ILogger<SetsEFRepository>? logger = null)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> Add(Set set)
        {
            _logger?.LogDebug("Adding set to database: {@Set}", set);
            try
            {
                SetEntity entity = _mapper.Map<SetEntity>(set);
                _context.Sets.Add(entity);
                await _context.SaveChangesAsync();
                _logger?.LogDebug("Set added to database successfully: {@Set}", set);
                return Result.Success();
            }
            catch (DbUpdateException exception)
            {
                _logger?.LogError(exception, "Database error occurred while adding set: {@Set}", set);
                return Result.Failure($"Failed to add set: {exception.Message}");
            }
        }

        public async Task<Result<Set>> GetByIdInExercise(Guid exerciseId, Guid id)
        {
            _logger?.LogDebug("Fetching from database set with ID: {SetId}", id);
            SetEntity? found = await _context.Sets.FirstOrDefaultAsync(s => s.id == id && s.exercise.id == exerciseId);
            if (found == null)
            {
                _logger?.LogWarning("Set with ID: {SetId} in exercise with ID: {ExerciseId} not found in database", id, exerciseId);
                return Result<Set>.NotFound($"Set with ID: {id} and exerciseID: {exerciseId} not found");
            }
            _logger?.LogDebug("Successfully fetched set from database: {@Set}", found);
            return Result<Set>.Success(_mapper.Map<Set>(found));
        }

        public async Task<Result<IEnumerable<Set>>> GetAllByExerciseId(Guid exerciseId)
        {
            _logger?.LogDebug("Fetching sets for exercise ID: {ExerciseId} from database", exerciseId);
            try
            {
                IEnumerable<SetEntity> found = await _context.Sets
                                                          .Where(s => s.exercise.id == exerciseId)
                                                          .ToListAsync();

                IEnumerable<Set> sets = _mapper.Map<IEnumerable<Set>>(found);
                _logger?.LogDebug("Successfully fetched sets for exercise ID: {ExerciseId}", exerciseId);
                return Result<IEnumerable<Set>>.Success(sets);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error fetching sets for exercise ID: {ExerciseId}", exerciseId);
                return Result<IEnumerable<Set>>.Failure($"Failed to fetch sets: {ex.Message}");
            }
        }

        public async Task<Result<Set>> Update(Set set)
        {
            _logger?.LogDebug("Updating set in database: {@Set}", set);
            SetEntity? entity = await _context.Sets.FirstOrDefaultAsync(s => s.id == set.Id && s.exercise.id == set.ExerciseId);
            if (entity == null)
            {
                _logger?.LogWarning("Set with ID: {SetId} and exerciseID: {ExerciseId} not found for update in database", set.Id, set.ExerciseId);
                return Result<Set>.NotFound($"Set with ID: {set.Id} and exerciseID: {set.ExerciseId} not found");
            }
            _mapper.Map(set, entity);
            try
            {
                _context.Sets.Update(entity);
                await _context.SaveChangesAsync();
                Set updated = _mapper.Map<Set>(entity);
                _logger?.LogDebug("Set updated successfully in database: {@Set}", updated);
                return Result<Set>.Success(updated);
            }
            catch (DbUpdateException exception)
            {
                _logger?.LogError(exception, "Database error occurred while updating set: {@Set}", set);
                return Result<Set>.Failure($"Failed to update set: {exception.Message}");
            }
        }

        public async Task<Result> DeleteFromExercise(Guid exerciseId, Guid id)
        {
            _logger?.LogDebug("Removing set with ID: {SetId} from database", id);
            var entity = await _context.Sets.FirstOrDefaultAsync(s => s.id == id && s.exercise.id == exerciseId);
            if (entity == null)
            {
                _logger?.LogWarning("Set with ID: {SetId} and exerciseID: {ExerciseId} not found for removal in database", id, exerciseId);
                return Result.NotFound($"Set with ID: {id} and exerciseID: {exerciseId} not found");
            }
            try
            {
                _context.Sets.Remove(entity);
                await _context.SaveChangesAsync();
                _logger?.LogDebug("Set removed successfully from database with ID: {SetId}", id);
                return Result.Success();
            }
            catch (DbUpdateException exception)
            {
                _logger?.LogError(exception, "Database error occurred while removing set with ID: {SetId}", id);
                return Result.Failure($"Failed to remove set: {exception.Message}");
            }
        }
    }
}
