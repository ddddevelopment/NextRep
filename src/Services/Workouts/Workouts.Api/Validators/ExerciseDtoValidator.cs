using FluentValidation;
using Workouts.Api.Models;

namespace Workouts.Api.Validators;

public class ExerciseDtoValidator : AbstractValidator<ExerciseDto>
{
    public ExerciseDtoValidator()
    {
        RuleFor(x => x.ExerciseInfo)
            .NotNull().WithMessage("ExerciseInfo is required");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Exercise name is required");
        RuleForEach(x => x.Sets)
            .SetValidator(new SetDtoValidator());
    }
}
