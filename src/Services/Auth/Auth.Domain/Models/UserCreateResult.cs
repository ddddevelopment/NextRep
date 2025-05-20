namespace Auth.Domain.Models;

public class UserCreateResult : UserResult
{
    public UserCreateResult(bool isSuccess, string? errorMessage = null) : base(isSuccess, errorMessage) { }
    
    public static UserCreateResult Success() => new UserCreateResult(true);
    public static UserCreateResult Failure(string errorMessage) => new UserCreateResult(false, errorMessage);
}