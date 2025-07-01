using Workouts.Domain.Models;

namespace Workouts.Domain.Services;

public interface IWorkoutsService
{
    Task<Result> Create(Workout workout);
    Task<Result<Workout>> GetById(Guid id);
    Task<Result<Workout>> Update(Workout workout);
    Task<Result> Delete(Guid id);
    Task<Result<IEnumerable<Workout>>> GetAllByUserId(Guid userId);
}
