using Workouts.Domain.Models;

namespace Workouts.Domain.Services;

public interface ISetsService
{
    Task<Result> Create(Guid workoutId, Set set);
    Task<Result<Set>> GetByIdInExercise(Guid workoutId, Guid exerciseId, Guid id);
    Task<Result<IEnumerable<Set>>> GetAllByExerciseId(Guid workoutId, Guid exerciseId);
    Task<Result<Set>> Update(Guid workoutId, Set set);
    Task<Result> DeleteFromExercise(Guid workoutId, Guid exerciseId, Guid id);
}
