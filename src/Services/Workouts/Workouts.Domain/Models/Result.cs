namespace Workouts.Domain.Models {
    public class Result {
        public bool IsSuccess { get; set; }
        public Error Error { get; set; }
        public static Result Success() => new Result { IsSuccess = true };
        public static Result Failure(string message) => new Result { IsSuccess = false, Error = new Error { Message = message } };
        public static Result<T> Success<T>(T value) => new Result<T> { IsSuccess = true, Value = value };
        public static Result<T> Failure<T>(string message) => new Result<T> { IsSuccess = false, Error = new Error { Message = message } };
    }
    public class Result<T> : Result {
        public T Value { get; set; }
    }
    public class Error {
        public string Message { get; set; }
    }
}
