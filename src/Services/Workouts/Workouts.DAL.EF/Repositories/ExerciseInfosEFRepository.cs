using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Workouts.DAL.EF.Entities;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;

namespace Workouts.DAL.EF.Repositories;

public class ExerciseInfosEFRepository : IExerciseInfosRepository
{
    private readonly WorkoutsDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ExerciseInfosEFRepository>? _logger;

    public ExerciseInfosEFRepository(WorkoutsDbContext context, IMapper mapper, ILogger<ExerciseInfosEFRepository>? logger = null)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result> Add(ExerciseInfo exerciseInfo)
    {
        _logger?.LogDebug("Adding ExerciseInfo to database: {@ExerciseInfo}", exerciseInfo);
        try
        {
            ExerciseInfoEntity entity = _mapper.Map<ExerciseInfoEntity>(exerciseInfo);
            _context.ExerciseInfos.Add(entity);
            await _context.SaveChangesAsync();
            _logger?.LogDebug("ExerciseInfo added to database successfully: {@ExerciseInfo}", exerciseInfo);
            return Result.Success();
        }
        catch (DbUpdateException exception)
        {
            _logger?.LogError(exception, "Database error occurred while adding exerciseInfo: {@ExerciseInfo}", exerciseInfo);
            return Result.Failure($"Failed to add exerciseInfo: {exception.Message}");
        }
    }

    public async Task<Result<ExerciseInfo>> GetById(Guid id)
    {
        _logger?.LogDebug("Fetching from database exerciseInfo with ID: {ExerciseInfoId}", id);
        ExerciseInfoEntity? found = await _context.ExerciseInfos.FindAsync(id);
        if (found == null)
        {
            _logger?.LogWarning("ExerciseInfo with ID: {ExerciseInfoId} not found in database", id);
            return Result<ExerciseInfo>.NotFound($"ExerciseInfo with ID: {id} not found");
        }
        _logger?.LogDebug("Successfully fetched exerciseInfo from database: {@ExerciseInfo}", found);
        return Result<ExerciseInfo>.Success(_mapper.Map<ExerciseInfo>(found));
    }

    public async Task<Result<IEnumerable<ExerciseInfo>>> GetAll()
    {
        _logger?.LogDebug("Fetching all exerciseInfos from database");
        IEnumerable<ExerciseInfoEntity> found = await _context.ExerciseInfos.ToListAsync();
        IEnumerable<ExerciseInfo> exerciseInfos = _mapper.Map<IEnumerable<ExerciseInfo>>(found);
        _logger?.LogDebug("Successfully fetched all exerciseInfos from database"); 
        return Result<IEnumerable<ExerciseInfo>>.Success(exerciseInfos);
    }

    public async Task<Result<ExerciseInfo>> Update(ExerciseInfo exerciseInfo)
    {
        _logger?.LogDebug("Updating exerciseInfo in database: {@ExerciseInfo}", exerciseInfo);
        ExerciseInfoEntity? entity = await _context.ExerciseInfos.FindAsync(exerciseInfo.Id);
        if (entity == null)
        {
            _logger?.LogWarning("ExerciseInfo with ID: {ExerciseInfoId} not found for update in database", exerciseInfo.Id);
            return Result<ExerciseInfo>.NotFound($"ExerciseInfo with ID: {exerciseInfo.Id} not found");
        }
        _mapper.Map(exerciseInfo, entity);
        try
        {
            _context.ExerciseInfos.Update(entity);
            await _context.SaveChangesAsync();
            ExerciseInfo updated = _mapper.Map<ExerciseInfo>(entity);
            _logger?.LogDebug("ExerciseInfo updated successfully in database: {@ExerciseInfo}", updated);
            return Result<ExerciseInfo>.Success(updated);
        }
        catch (DbUpdateException exception)
        {
            _logger?.LogError(exception, "Database error occurred while updating exerciseInfo: {@ExerciseInfo}", exerciseInfo);
            return Result<ExerciseInfo>.Failure($"Failed to update exerciseInfo: {exception.Message}");
        }
    }

    public async Task<Result> Delete(Guid id)
    {
        _logger?.LogDebug("Removing exerciseInfo with ID: {ExerciseInfoId} from database", id);
        var entity = await _context.ExerciseInfos.FindAsync(id);
        if (entity == null)
        {
            _logger?.LogWarning("ExerciseInfo with ID: {ExerciseInfoId} not found for removal in database", id);
            return Result.NotFound($"ExerciseInfo with ID: {id} not found");
        }
        try
        {
            _context.ExerciseInfos.Remove(entity);
            await _context.SaveChangesAsync();
            _logger?.LogDebug("ExerciseInfo removed successfully from database with ID: {ExerciseInfoId}", id);
            return Result.Success();
        }
        catch (DbUpdateException exception)
        {
            _logger?.LogError(exception, "Database error occurred while removing exerciseInfo with ID: {ExerciseInfoId}", id);
            return Result.Failure($"Failed to remove exerciseInfo: {exception.Message}");
        }
    }
}