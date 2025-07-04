using FluentValidation;
using Workouts.Api.Models;

namespace Workouts.Api.Validators;

public class WorkoutBaseValidator<T> : AbstractValidator<T> where T : WorkoutBase
{
    public WorkoutBaseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot be longer than 100 characters");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("StartTime is required");
        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("EndTime is required");
        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime)
            .WithMessage("EndTime must be after StartTime");
    }
}