using FluentValidation;
using Workouts.Api.Models;

namespace Workouts.Api.Validators;

public class WorkoutDtoValidator : WorkoutBaseValidator<WorkoutDto> {
    public WorkoutDtoValidator() : base()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}
