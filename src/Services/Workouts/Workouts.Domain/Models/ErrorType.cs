namespace Workouts.Domain.Models;

public enum ErrorType
{
    NotFound,
    Validation,
    Conflict,
    Forbidden,
    InternalServerError,
    BadRequest
}