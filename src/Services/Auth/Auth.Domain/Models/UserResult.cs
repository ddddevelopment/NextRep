namespace Auth.Domain.Models;

public abstract class UserResult {
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public UserResult(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }
}