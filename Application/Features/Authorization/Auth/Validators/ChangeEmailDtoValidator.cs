using FluentValidation;
using MicroERP.Application.Features.Authentication.Auth.DTOs;

namespace MicroERP.Application.Features.Authentication.Auth.Validators;

public class ChangeEmailDtoValidator : AbstractValidator<ChangeEmailDto>
{
    public ChangeEmailDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}