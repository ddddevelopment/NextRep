using Users.Domain.Models;

namespace Users.Domain.Repositories {
    public interface IUsersRepository {
        Task Add(User user);
        Task<User> Get(Guid id);
        Task<IEnumerable<User>> GetAll();
        Task<User> Update(User user);
        Task Remove(Guid id);
    }
}