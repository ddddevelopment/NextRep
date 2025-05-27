using FluentValidation;
using Workouts.Api.Models;
using Workouts.Domain.Models;

namespace Workouts.Api.Validators;

public class ExerciseInfoDtoValidator : ExerciseInfoBaseValidator<ExerciseInfoDto>
{
    public ExerciseInfoDtoValidator() : base()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}
