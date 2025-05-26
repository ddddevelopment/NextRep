using Workouts.Domain.Models;

namespace Workouts.Domain.Services;

public interface IExerciseInfosService
{
    Task<Result> Create(ExerciseInfo exerciseInfo);
    Task<Result<IEnumerable<ExerciseInfo>>> GetAll();
    Task<Result<ExerciseInfo>> GetById(Guid id);
    Task<Result<ExerciseInfo>> Update(ExerciseInfo exerciseInfo);
    Task<Result> Delete(Guid id);
}
