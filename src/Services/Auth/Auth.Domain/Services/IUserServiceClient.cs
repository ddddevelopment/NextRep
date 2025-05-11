using Auth.Domain.Models;

namespace Auth.Domain.Services;

public interface IUsersServiceClient {
    Task<UserDto> GetUserByEmail(string email);
}