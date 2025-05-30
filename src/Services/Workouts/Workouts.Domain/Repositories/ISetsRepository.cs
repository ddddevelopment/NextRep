using Workouts.Domain.Models;

namespace Workouts.Domain.Repositories;

public interface ISetsRepository
{
    Task<Result> Add(Set set);
    Task<Result<Set>> GetByIdInExercise(Guid exerciseId, Guid id);
    Task<Result<IEnumerable<Set>>> GetAllByExerciseId(Guid exerciseId);
    Task<Result<Set>> Update(Set set);
    Task<Result> DeleteFromExercise(Guid exerciseId, Guid id);
}
