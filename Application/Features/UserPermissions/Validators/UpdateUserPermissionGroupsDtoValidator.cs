using FluentValidation;
using MicroERP.Application.Features.UserPermissions.DTOs;

namespace MicroERP.Application.Features.UserPermissions.Validators;

public class UpdateUserPermissionGroupsDtoValidator
    : AbstractValidator<UpdateUserPermissionGroupsDto>
{
    public UpdateUserPermissionGroupsDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();


        RuleFor(x => x.PermissionGroupKeys)
            .NotNull();


        RuleForEach(x => x.PermissionGroupKeys)
            .NotEmpty();


        RuleFor(x => x.PermissionGroupKeys)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage(
                "Duplicate permission groups are not allowed.");
    }
}