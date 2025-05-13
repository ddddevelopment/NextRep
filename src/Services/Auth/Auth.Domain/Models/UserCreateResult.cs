namespace Auth.Domain.Models;

public class UserCreateResult {
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }

    public static UserCreateResult Success() => new UserCreateResult() { IsSuccess = true };
    public static UserCreateResult Failure(string errorMessage) => new UserCreateResult() { IsSuccess = false, ErrorMessage = errorMessage };
}