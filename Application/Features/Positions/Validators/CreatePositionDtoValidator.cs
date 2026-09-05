using FluentValidation;
using MicroERP.Application.Features.Positions.DTOs;

namespace MicroERP.Application.Features.Positions.Validators;

public class CreatePositionDtoValidator : AbstractValidator<CreatePositionDto>
{
    public CreatePositionDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.NameAr)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.NameEn)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
