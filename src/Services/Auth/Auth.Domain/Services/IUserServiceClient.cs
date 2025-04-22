using Auth.Domain.Models;

namespace Auth.Domain.Services;

public interface IUserServiceClient {
    Task<UserDto> GetUser(Guid userId);
    Task<UserDto> GetUserByEmail(string email);
    Task<UserDto> GetUserByUsername(string username);
    Task<IEnumerable<UserDto>> GetAllUsers();
    Task<UserDto> CreateUser(UserDto user);
    Task<UserDto> UpdateUser(UserDto user);
    Task DeleteUser(Guid userId);
}
