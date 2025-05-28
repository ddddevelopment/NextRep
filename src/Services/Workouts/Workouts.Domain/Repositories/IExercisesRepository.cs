using Workouts.Domain.Models;

namespace Workouts.Domain.Repositories;

public interface IExercisesRepository
{
    Task<Result> Add(Exercise exercise);
    Task<Result<Exercise>> GetById(Guid id);
    Task<Result<IEnumerable<Exercise>>> GetByWorkoutId(Guid workoutId); // Получение упражнений для конкретной тренировки
    Task<Result<Exercise>> Update(Exercise exercise);
    Task<Result> Delete(Guid id);
    // Возможно, понадобится метод GetAll(), если нужно получать все упражнения независимо от тренировки,
    // но GetByWorkoutId() кажется более приоритетным для вашего случая.
    // Task<Result<IEnumerable<Exercise>>> GetAll(); 
} 