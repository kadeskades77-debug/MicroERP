using FluentValidation;
using MicroERP.Application.Features.Positions.DTOs;

namespace MicroERP.Application.Features.Positions.Validators;

public class UpdatePositionDtoValidator : AbstractValidator<UpdatePositionDto>
{
    public UpdatePositionDtoValidator()
    {
        RuleFor(x => x.Code)
            .MaximumLength(50)
            .When(x => x.Code is not null);

        RuleFor(x => x.NameAr)
            .MaximumLength(150)
            .When(x => x.NameAr is not null);

        RuleFor(x => x.NameEn)
            .MaximumLength(150)
            .When(x => x.NameEn is not null);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
