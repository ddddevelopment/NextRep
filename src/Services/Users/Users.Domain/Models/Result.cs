namespace Users.Domain.Models;

public class Result<T> {
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public Error? Error { get; set; }

    private Result(bool isSuccess, T? value = default, Error? error = null) {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new Result<T>(true, value: value);
    public static Result<T> Failure(string? errorMessage = null) => new Result<T>(false, error: new Error(ErrorType.Unknown, message: errorMessage));
    public static Result<T> NotFound(string? errorMessage = null) => new Result<T>(false, error: new Error(ErrorType.NotFound, message: errorMessage));
    public static Result<T> Conflict(string? errorMessage = null) => new Result<T>(false, error: new Error(ErrorType.Conflict, message: errorMessage));
    public static Result<T> Invalid(string? errorMessage = null) => new Result<T>(false, error: new Error(ErrorType.Validation, message: errorMessage)); 
}

public class Result {
    public bool IsSuccess { get; set; }
    public Error? Error { get; set; }

    private Result(bool isSuccess, Error? error = null)  {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new Result(true, null);
    public static Result Failure(string? errorMessage) => new Result(false, error: new Error(ErrorType.Unknown, message: errorMessage));
    public static Result NotFound(string? errorMessage) => new Result(false, error: new Error(ErrorType.NotFound, message: errorMessage));
    public static Result Conflict(string? errorMessage) => new Result(false, error: new Error(ErrorType.Conflict, message: errorMessage));
    public static Result Invalid(string? errorMessage) => new Result(false, error: new Error(ErrorType.Validation, message: errorMessage)); 
}