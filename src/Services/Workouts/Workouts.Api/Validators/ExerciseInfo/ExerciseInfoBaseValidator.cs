using FluentValidation;
using Workouts.Api.Models;
using Workouts.Domain.Models;

namespace Workouts.Api.Validators;

public abstract class ExerciseInfoBaseValidator<T> : AbstractValidator<T> where T : ExerciseInfoBase
{
    public ExerciseInfoBaseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.MuscleGroup)
            .NotEmpty()
            .WithMessage("MuscleGroup is required.")
            .Must(BeAValidMuscleGroup)
            .WithMessage($"MuscleGroup must be one of: {string.Join(", ", Enum.GetNames(typeof(MuscleGroup)))}");
    }

    private bool BeAValidMuscleGroup(string value)
    {
        return Enum.TryParse<MuscleGroup>(value, true, out _);
    }
}