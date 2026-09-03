using FluentValidation;
using MicroERP.Application.Features.Authorization.Permissions.DTOs;

namespace MicroERP.Application.Features.Authorization.Permissions.Validators;

public class CreatePermissionDtoValidator
    : AbstractValidator<CreatePermissionDto>
{
    public CreatePermissionDtoValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}