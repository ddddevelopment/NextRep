namespace Auth.Domain.Models;

public class UserGetResult : UserResult {
    public bool IsFound { get; set; }
    public UserDto? User { get; set; }

    public UserGetResult(bool isFound, UserDto? user = default, bool isSuccess = true, string? errorMessage = null) : base(isSuccess, errorMessage)
    {
        IsFound = isFound;
        User = user;
    }

    public static UserGetResult Found(UserDto user) => new UserGetResult(true, user: user);
    public static UserGetResult NotFound() => new UserGetResult(false);
    public static UserGetResult Failure(string errorMessage) => new UserGetResult(false, isSuccess: false, errorMessage: errorMessage);
}