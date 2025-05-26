using Microsoft.Extensions.Logging;
using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;

namespace Workouts.Application.Services;

public class ExerciseInfosService : IExerciseInfosService
{
    private readonly IExerciseInfosRepository _repository;
    private readonly ILogger<ExerciseInfosService>? _logger;

    public ExerciseInfosService(IExerciseInfosRepository repository, ILogger<ExerciseInfosService>? logger = null)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result> Create(ExerciseInfo exerciseInfo)
    {
        _logger?.LogDebug("Attempting to create exerciseInfo: {@ExerciseInfo}", exerciseInfo);

        if (exerciseInfo == null)
        {
            _logger?.LogWarning("ExerciseInfo is null");
            return Result.Invalid("ExerciseInfo must not be null");
        }

        Result result = await _repository.Add(exerciseInfo);

        if (result.IsSuccess)
        {
            _logger?.LogInformation("ExerciseInfo created successfully: {@ExerciseInfo}", exerciseInfo);
        }

        return result;
    }

    public async Task<Result<ExerciseInfo>> GetById(Guid id)
    {
        _logger?.LogDebug("Fetching ExerciseInfo with ID: {ExerciseInfoId}", id);

        Result<ExerciseInfo> result = await _repository.GetById(id);

        if (result.IsSuccess)
        {
            _logger?.LogInformation("ExerciseInfo retrieved successfully: {@ExerciseInfo}", result.Value);
        }

        return result;
    }

    public async Task<Result<IEnumerable<ExerciseInfo>>> GetAll()
    {
        _logger?.LogDebug("Fetching all exerciseInfos");

        Result<IEnumerable<ExerciseInfo>> result = await _repository.GetAll();

        if (result.IsSuccess)
        {
            _logger?.LogInformation("Successfully retrieved all exerciseInfos");
        }

        return result;
    }

    public async Task<Result<ExerciseInfo>> Update(ExerciseInfo exerciseInfo)
    {
        _logger?.LogDebug("Attempting to update exerciseInfo: {@ExerciseInfo}", exerciseInfo);

        if (exerciseInfo == null)
        {
            _logger?.LogWarning("ExerciseInfo is null");
            return Result<ExerciseInfo>.Invalid("ExerciseInfo must not be null");
        }

        Result<ExerciseInfo> result = await _repository.Update(exerciseInfo);

        if (result.IsSuccess)
        {
            _logger?.LogInformation("ExerciseInfo updated successfully: {@ExerciseInfo}", result.Value);
        }

        return result;
    }

    public async Task<Result> Delete(Guid id)
    {
        _logger?.LogDebug("Attempting to delete exerciseInfo with ID: {ExerciseInfoId}", id);

        Result result = await _repository.Delete(id);

        if (result.IsSuccess)
        {
            _logger?.LogInformation("ExerciseInfo deleted successfully with ID: {ExerciseInfoId}", id);
        }

        return result;
    }
}