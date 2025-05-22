using Workouts.Domain.Models;

namespace Workouts.Domain.Services {
    public interface IWorkoutsService {
        Task<Result> Create(Workout workout);
        Task<Result<Workout>> GetById(Guid id);
        Task<Result> Update(Workout workout);
        Task<Result> Delete(Guid id);
    }
}
