using FluentValidation;
using Workouts.Api.Models;

namespace Workouts.Api.Validators;

public class ExerciseInfoRequestValidator : ExerciseInfoBaseValidator<ExerciseInfoCreateRequest> { }