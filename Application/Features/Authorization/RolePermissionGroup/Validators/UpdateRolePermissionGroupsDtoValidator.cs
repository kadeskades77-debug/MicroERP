using FluentValidation;
using MicroERP.Application.Features.Authorization.RolePermissionGroup.DTOs;


namespace MicroERP.Application.Features.Authorization.RolePermissionGroup.Validators
{
    public class UpdateRolePermissionGroupsDtoValidator
        : AbstractValidator<UpdateRolePermissionGroupsDto>
    {
        public UpdateRolePermissionGroupsDtoValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty()
                .WithMessage("RoleId is required.");

            RuleFor(x => x.PermissionGroupKeys)
                .NotNull()
                .WithMessage(
                    "PermissionGroupKeys cannot be null.");

            RuleForEach(x => x.PermissionGroupKeys)
                .NotEmpty()
                .WithMessage(
                    "Permission group key cannot be empty.");
        }
    }
}
