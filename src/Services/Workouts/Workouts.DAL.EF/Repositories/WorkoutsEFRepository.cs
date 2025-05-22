using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Workouts.DAL.Entities;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;

namespace Workouts.DAL.Repositories
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
                return Result.Failure<Workout>("Workout not found");
            }
            return Result.Success(_mapper.Map<Workout>(found));
        }

        public async Task<Result> Update(Workout workout)
        {
            var entity = await _context.Workouts.FindAsync(workout.Id);
            if (entity == null)
                return Result.Failure("Workout not found");
            _mapper.Map(workout, entity);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> Delete(Guid id)
        {
            var entity = await _context.Workouts.FindAsync(id);
            if (entity == null)
                return Result.Failure("Workout not found");
            _context.Workouts.Remove(entity);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
    }
}
