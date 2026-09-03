using FluentValidation;
using MicroERP.Application.Features.Authorization.PermissionGroups.DTOs;

namespace MicroERP.Application.Features.Authorization.PermissionGroups.Validators;

public class CreatePermissionGroupDtoValidator
    : AbstractValidator<CreatePermissionGroupDto>
{
    public CreatePermissionGroupDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Key)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

    }
}