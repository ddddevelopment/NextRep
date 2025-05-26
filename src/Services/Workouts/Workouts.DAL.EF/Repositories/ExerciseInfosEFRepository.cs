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

    public ExerciseInfosEFRepository(WorkoutsDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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

    public Task<Result<IEnumerable<ExerciseInfo>>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<Result<ExerciseInfo>> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ExerciseInfo>> Update(ExerciseInfo exerciseInfo)
    {
        throw new NotImplementedException();
    }

    public Task<Result> Delete(Guid id)
    {
        throw new NotImplementedException();
    }
}