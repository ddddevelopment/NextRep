using FluentValidation;
using Workouts.Api.Models;

namespace Workouts.Api.Validators;

public class SetDtoValidator : AbstractValidator<SetDto>
{
    public SetDtoValidator()
    {
        RuleFor(x => x.Reps)
            .GreaterThan(0).WithMessage("Reps must be greater than 0");
        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0).WithMessage("Weight must be non-negative");
    }
}
