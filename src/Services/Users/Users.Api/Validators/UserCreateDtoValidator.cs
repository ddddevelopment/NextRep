using FluentValidation;
using Users.Api.Models;

namespace Users.Api.Validators;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto> {
    public UserCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Telephone)
            .NotEmpty().WithMessage("Telephone is required")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid telephone number");
    }
}