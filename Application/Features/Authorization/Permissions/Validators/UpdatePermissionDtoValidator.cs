using FluentValidation;
using MicroERP.Application.Features.Authorization.Permissions.DTOs;

namespace MicroERP.Application.Features.Authorization.Permissions.Validators;

public class UpdatePermissionDtoValidator
    : AbstractValidator<UpdatePermissionDto>
{
    public UpdatePermissionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}