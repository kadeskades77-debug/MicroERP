using FluentValidation;
using MicroERP.Application.Features.Authorization.RolePermissionGroup.DTOs;


namespace MicroERP.Application.Features.Authorization.RolePermissionGroup.Validators
{
    public class RemoveRolePermissionGroupDtoValidator
        : AbstractValidator<RemoveRolePermissionGroupDto>
    {
        public RemoveRolePermissionGroupDtoValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty()
                .WithMessage("RoleId is required.");

            RuleFor(x => x.PermissionGroupKey)
                .NotEmpty()
                .WithMessage(
                    "Permission group key is required.");
        }
    }
}
