using Users.Domain.Models;
using Users.Domain.Repositories;

namespace Users.Domain.Services {
    public interface IUsersService {
        Task Create(User user);
        Task<User> Get(Guid id);
        Task<IEnumerable<User>> GetAll();
        Task<User> Update(User user);
    }
}