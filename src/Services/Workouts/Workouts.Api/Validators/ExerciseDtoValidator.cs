using FluentValidation;
using Workouts.Api.Models;

namespace Workouts.Api.Validators;

public class ExerciseDtoValidator : AbstractValidator<ExerciseDto>
{
    public ExerciseDtoValidator()
    {
        RuleFor(x => x.ExerciseInfoId)
            .NotEmpty().WithMessage("ExerciseInfoId is required");
        RuleFor(x => x.Sets)
            .NotEmpty().WithMessage("Sets are required");
        RuleForEach(x => x.Sets)
            .SetValidator(new SetDtoValidator());
    }
}