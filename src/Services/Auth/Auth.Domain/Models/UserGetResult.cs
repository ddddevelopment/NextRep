namespace Auth.Domain.Models;

public class UserGetResult {
    public bool Found { get; set; }
    public UserDto? User { get; set; }
    public string? ErrorMessage { get; set; }

    public static UserGetResult Success(UserDto user) => new UserGetResult() { Found = true, User = user };
    public static UserGetResult Failure(string errorMessage) => new UserGetResult() { Found = false, ErrorMessage = errorMessage };
}