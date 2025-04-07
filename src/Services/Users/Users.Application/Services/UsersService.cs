using Users.Domain.Models;
using Users.Domain.Repositories;
using Users.Domain.Services;

namespace Users.Application.Services {
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;

        public UsersService(IUsersRepository repository)
        {   
            _repository = repository;    
        }

        public async Task Create(User user)
        {
            await _repository.Add(user);
        }

        public async Task<User> Get(Guid id)
        {
            return await _repository.Get(id);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<User> Update(User user)
        {
            return await _repository.Update(user);
        }
    }
}