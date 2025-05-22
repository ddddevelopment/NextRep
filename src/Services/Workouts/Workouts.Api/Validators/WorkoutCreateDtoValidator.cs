using FluentValidation;
using Workouts.Api.Models;

namespace Workouts.Api.Validators;

public class WorkoutCreateDtoValidator : AbstractValidator<WorkoutCreateRequest> {
    public WorkoutCreateDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required");
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required")
            .MaximumLength(50).WithMessage("Type must not exceed 50 characters");
        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than 0");
        RuleFor(x => x.CaloriesBurned)
            .GreaterThanOrEqualTo(0).WithMessage("Calories burned must be non-negative");
    }
}
