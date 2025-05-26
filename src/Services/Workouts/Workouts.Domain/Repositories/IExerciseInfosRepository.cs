using Workouts.Domain.Models;

namespace Workouts.Domain.Repositories;

public interface IExerciseInfosRepository
{
    Task<Result> Add(ExerciseInfo exerciseInfo);
    Task<Result<ExerciseInfo>> GetById(Guid id);
    Task<Result<IEnumerable<ExerciseInfo>>> GetAll();
    Task<Result<ExerciseInfo>> Update(ExerciseInfo exerciseInfo);
    Task<Result> Delete(Guid id);
}