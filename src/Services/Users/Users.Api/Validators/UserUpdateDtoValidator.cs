using FluentValidation;
using Users.Api.Models;

namespace Users.Api.Validators;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto> {
    public UserUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required")
            .Must(id => id != Guid.Empty).WithMessage("Id must be a valid non-empty GUID");

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