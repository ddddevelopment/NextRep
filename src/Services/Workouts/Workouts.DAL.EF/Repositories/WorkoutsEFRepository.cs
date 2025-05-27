using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Workouts.DAL.EF.Entities;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;

namespace Workouts.DAL.EF.Repositories
{
    public class WorkoutsEFRepository : IWorkoutsRepository
    {
        private readonly WorkoutsDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<WorkoutsEFRepository>? _logger;

        public WorkoutsEFRepository(WorkoutsDbContext context, IMapper mapper, ILogger<WorkoutsEFRepository>? logger = null)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> Add(Workout workout)
        {
            _logger?.LogDebug("Adding workout to database: {@Workout}", workout);
            try
            {
                WorkoutEntity entity = _mapper.Map<WorkoutEntity>(workout);
                _context.Workouts.Add(entity);
                await _context.SaveChangesAsync();
                _logger?.LogDebug("Workout added to database successfully: {@Workout}", workout);
                return Result.Success();
            }
            catch (DbUpdateException exception)
            {
                _logger?.LogError(exception, "Database error occurred while adding workout: {@Workout}", workout);
                return Result.Failure($"Failed to add workout: {exception.Message}");
            }
        }

        public async Task<Result<Workout>> GetById(Guid id)
        {
            _logger?.LogDebug("Fetching from database workout with ID: {WorkoutId}", id);
            WorkoutEntity? found = await _context.Workouts.FindAsync(id);
            if (found == null)
            {
                _logger?.LogWarning("Workout with ID: {WorkoutId} not found in database", id);
                return Result<Workout>.NotFound($"Workout with ID: {id} not found");
            }
            _logger?.LogDebug("Successfully fetched workout from database: {@Workout}", found);
            return Result<Workout>.Success(_mapper.Map<Workout>(found));
        }

        public async Task<Result<IEnumerable<Workout>>> GetAll()
        {
            _logger?.LogDebug("Fetching all workouts from database");

            IEnumerable<WorkoutEntity> found = await _context.Workouts.ToListAsync();
            
            IEnumerable<Workout> workouts = _mapper.Map<IEnumerable<Workout>>(found);
            _logger?.LogDebug("Successfully fetched all workouts from database");
            return Result<IEnumerable<Workout>>.Success(workouts);
        }

        public async Task<Result<Workout>> Update(Workout workout)
        {
            _logger?.LogDebug("Updating workout in database: {@Workout}", workout);
            WorkoutEntity? entity = await _context.Workouts.FindAsync(workout.Id);
            if (entity == null)
            {
                _logger?.LogWarning("Workout with ID: {WorkoutId} not found for update in database", workout.Id);
                return Result<Workout>.NotFound($"Workout with ID: {workout.Id} not found");
            }
            _mapper.Map(workout, entity);
            try
            {
                _context.Workouts.Update(entity);
                await _context.SaveChangesAsync();
                Workout updated = _mapper.Map<Workout>(entity);
                _logger?.LogDebug("Workout updated successfully in database: {@Workout}", updated);
                return Result<Workout>.Success(updated);
            }
            catch (DbUpdateException exception)
            {
                _logger?.LogError(exception, "Database error occurred while updating workout: {@Workout}", workout);
                return Result<Workout>.Failure($"Failed to update workout: {exception.Message}");
            }
        }

        public async Task<Result> Delete(Guid id)
        {
            _logger?.LogDebug("Removing workout with ID: {WorkoutId} from database", id);
            var entity = await _context.Workouts.FindAsync(id);
            if (entity == null)
            {
                _logger?.LogWarning("Workout with ID: {WorkoutId} not found for removal in database", id);
                return Result.NotFound($"Workout with ID: {id} not found");
            }
            try
            {
                _context.Workouts.Remove(entity);
                await _context.SaveChangesAsync();
                _logger?.LogDebug("Workout removed successfully from database with ID: {WorkoutId}", id);
                return Result.Success();
            }
            catch (DbUpdateException exception)
            {
                _logger?.LogError(exception, "Database error occurred while removing workout with ID: {WorkoutId}", id);
                return Result.Failure($"Failed to remove workout: {exception.Message}");
            }
        }
    }
}
