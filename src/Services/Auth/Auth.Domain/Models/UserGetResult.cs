namespace Auth.Domain.Models;

public class UserGetResult {
    public bool IsSuccess { get; set; }
    public UserDto? User { get; set; }
    public string? ErrorMessage { get; set; }

    public static UserGetResult Success(UserDto user) => new UserGetResult() { IsSuccess = true, User = user };
    public static UserGetResult Failure(string errorMessage) => new UserGetResult() { IsSuccess = false, ErrorMessage = errorMessage };
}