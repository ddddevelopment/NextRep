namespace Workouts.Domain.Models {
    public class Result
    {
        public bool IsSuccess { get; set; }
        public Error? Error { get; set; }

        public Result(bool isSuccess, Error? error = null)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true);
        public static Result Failure(string message) => new Result(false, new Error(ErrorType.Unknown, message: message));
        public static Result NotFound(string message) => new Result(false, new Error(ErrorType.NotFound, message: message));
        public static Result Conflict(string message) => new Result(false, new Error(ErrorType.Conflict, message: message)); 
        public static Result Invalid(string message) => new Result(false, new Error(ErrorType.Validation, message: message));       
    }
    
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Value { get; set; }
        public Error? Error { get; set; }

        public Result(bool isSuccess, T? value = default, Error? error = null)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }
        public static Result<T> Success(T value) => new Result<T>(true, value: value);
        public static Result<T> Failure(string message) => new Result<T>(false, error: new Error(ErrorType.Unknown, message: message));
        public static Result<T> NotFound(string message) => new Result<T>(false, error: new Error(ErrorType.NotFound, message: message));
        public static Result<T> Invalid(string message) => new Result<T>(false, error: new Error(ErrorType.Validation, message: message));
    }
}
