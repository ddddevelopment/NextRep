using Auth.Domain.Models;

namespace Auth.Domain.Services;

public interface IUserServiceClient {
    Task<UserDto> GetUserByEmail(string email);
}