using FluentValidation;
using Workouts.Api.Models;

namespace Workouts.Api.Validators;

public class ExerciseCreateRequestForWorkoutValidator : AbstractValidator<ExerciseCreateRequestForWorkout>
{
    public ExerciseCreateRequestForWorkoutValidator()
    {
        RuleFor(x => x.ExerciseInfoId)
            .NotEmpty().WithMessage("ExerciseInfoId is required");
    }
}
