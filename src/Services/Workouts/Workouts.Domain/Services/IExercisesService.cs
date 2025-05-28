using Workouts.Domain.Models;

namespace Workouts.Domain.Services;

public interface IExercisesService
{
    Task<Result> Create(Exercise exercise);
    Task<Result<Exercise>> GetById(Guid id);
    Task<Result<IEnumerable<Exercise>>> GetByWorkoutId(Guid workoutId);
    Task<Result<Exercise>> Update(Exercise exercise);
    Task<Result> Delete(Guid id);
    // Возможно, понадобится метод GetAll(), если нужно получать все упражнения независимо от тренировки
    // Task<Result<IEnumerable<Exercise>>> GetAll(); 
} 