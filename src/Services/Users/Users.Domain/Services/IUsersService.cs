using Users.Domain.Models;
using Users.Domain.Repositories;

namespace Users.Domain.Services {
    public interface IUsersService {
        Task Create(User user);
        Task<User> GetById(Guid id);
        Task<User> GetByEmail(string email);
        Task<IEnumerable<User>> GetAll();
        Task<User> Update(User user);
        Task Delete(Guid id);
    }
}