using Workouts.Domain.Models;

namespace Workouts.Domain.Repositories {
    public interface IWorkoutsRepository {
        Task<Result> Add(Workout workout);
        Task<Result<Workout>> GetById(Guid id);
        Task<Result<IEnumerable<Workout>>> GetAllByUserId(Guid userId);
        Task<Result<Workout>> Update(Workout workout);
        Task<Result> Delete(Guid id);
    }
}
