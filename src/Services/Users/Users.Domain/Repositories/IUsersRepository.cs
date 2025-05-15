using Users.Domain.Models;

namespace Users.Domain.Repositories {
    public interface IUsersRepository {
        Task<Result> Add(User user);
        Task<Result<User>> GetById(Guid id);
        Task<Result<User>> GetByEmail(string email);
        Task<Result<IEnumerable<User>>> GetAll();
        Task<Result<User>> Update(User user);
        Task<Result> Remove(Guid id);
        Task<bool> ExistsByEmail(string email);
    }
}