using Workouts.Domain.Models;

namespace Workouts.Domain.Repositories;

public interface IExercisesRepository
{
    Task<Result> Add(Exercise exercise);
    Task<Result<Exercise>> GetByIdInWorkout(Guid workoutId, Guid id);
    Task<Result<IEnumerable<Exercise>>> GetAllByWorkoutId(Guid workoutId); 
    Task<Result<Exercise>> Update(Exercise exercise);
    Task<Result> DeleteFromWorkout(Guid workoutId, Guid id);
    Task<Result<Exercise>> GetById(Guid id); 
}