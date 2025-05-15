using Users.Domain.Models;
using Users.Domain.Repositories;

namespace Users.Domain.Services {
    public interface IUsersService {
        Task<Result> Create(User user);
        Task<Result<User>> GetById(Guid id);
        Task<Result<User>> GetByEmail(string email);
        Task<Result<IEnumerable<User>>> GetAll();
        Task<Result<User>> Update(User user);
        Task<Result> Delete(Guid id);
    }
}