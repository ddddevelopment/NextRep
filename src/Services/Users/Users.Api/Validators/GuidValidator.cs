using System.Data;
using FluentValidation;

namespace Users.Api.Validators;

public class GuidValidator : AbstractValidator<Guid> {
    public GuidValidator()
    {  
        RuleFor(id => id)
            .NotEmpty().WithMessage("Id is required")
            .Must(id => id != Guid.Empty).WithMessage("Id must be a valid non-empty GUID");
    }
}