using FluentValidation;
using MicroERP.Application.Features.PermissionGroups.DTOs;

namespace MicroERP.Application.Features.PermissionGroups.Validators;

public class UpdatePermissionGroupDtoValidator
    : AbstractValidator<UpdatePermissionGroupDto>
{
    public UpdatePermissionGroupDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100);

        RuleFor(x => x.Key)
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

      }
}