using FluentValidation;
using Workouts.Api.Models;

namespace Workouts.Api.Validators;

public class WorkoutUpdateDtoValidator : AbstractValidator<WorkoutUpdateDto> {
    public WorkoutUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("StartTime is required");
        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("EndTime is required");
        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime)
            .WithMessage("EndTime must be after StartTime");
        RuleForEach(x => x.Exercises)
            .SetValidator(new ExerciseDtoValidator());
    }
}
