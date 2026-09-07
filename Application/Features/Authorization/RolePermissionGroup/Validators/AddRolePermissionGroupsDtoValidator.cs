using FluentValidation;
using MicroERP.Application.Features.Authorization.RolePermissionGroup.DTOs;


namespace MicroERP.Application.Features.Authorization.RolePermissionGroup.Validators
{
   

    public class AddRolePermissionGroupsDtoValidator
        : AbstractValidator<AddRolePermissionGroupsDto>
    {
        public AddRolePermissionGroupsDtoValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty()
                .WithMessage("RoleId is required.");

            RuleFor(x => x.PermissionGroupKeys)
                .NotNull()
                .Must(x => x.Count > 0)
                .WithMessage(
                    "At least one permission group is required.");

            RuleForEach(x => x.PermissionGroupKeys)
                .NotEmpty()
                .WithMessage(
                    "Permission group key cannot be empty.");
        }
    }
}
