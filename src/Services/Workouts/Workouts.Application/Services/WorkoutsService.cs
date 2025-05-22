using Workouts.Domain.Models;
using Workouts.Domain.Repositories;
using Workouts.Domain.Services;

namespace Workouts.Application.Services {
    public class WorkoutsService : IWorkoutsService {
        private readonly IWorkoutsRepository _repository;
        public WorkoutsService(IWorkoutsRepository repository) {
            _repository = repository;
        }
        public async Task<Result> Create(Workout workout) => await _repository.Add(workout);
        public async Task<Result<Workout>> GetById(Guid id) => await _repository.GetById(id);
        public async Task<Result> Update(Workout workout) => await _repository.Update(workout);
        public async Task<Result> Delete(Guid id) => await _repository.Delete(id);
    }
}
