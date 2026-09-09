using FluentValidation;
using MicroERP.Application.Features.Authorization.Auth.DTOs;

namespace MicroERP.Application.Features.Authorization.Auth.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(100);
    }
}