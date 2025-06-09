namespace Auth.Domain.Models;

public class UserCreateResult : UserResult
{
    public Guid? Id { get; set; }
    public UserCreateResult(bool isSuccess, Guid? id = null, string? errorMessage = null) : base(isSuccess, errorMessage)
    {
        Id = id;
    }

    public static UserCreateResult Success(Guid? id) => new UserCreateResult(true, id);
    public static UserCreateResult Failure(string errorMessage) => new UserCreateResult(false, errorMessage: errorMessage);
}