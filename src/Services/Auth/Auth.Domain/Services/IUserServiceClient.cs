using Auth.Domain.Models;

namespace Auth.Domain.Services;

public interface IUsersServiceClient {
    Task<UserGetResult> GetUserByEmail(string email);
    Task<UserCreateResult> CreateUser(UserDto user);
}