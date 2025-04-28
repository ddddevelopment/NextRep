using Users.Domain.Models;

namespace Users.Domain.Repositories {
    public interface IUsersRepository {
        Task Add(User user);
        Task<User> GetById(Guid id);
        Task<User> GetByEmail(string email);
        Task<IEnumerable<User>> GetAll();
        Task<User> Update(User user);
        Task Remove(Guid id);
        Task<bool> ExistsByEmail(string email);
    }
}