using FluentValidation;
using MicroERP.Application.Features.Payrolls.SalaryComponents.DTOs;

namespace MicroERP.Application.Features.Payrolls.SalaryComponents.Validators;

public class UpdateSalaryComponentValidator
    : AbstractValidator<UpdateSalaryComponentDto>
{
    public UpdateSalaryComponentValidator()
    {
        RuleFor(x => x.NameAr)
            .MaximumLength(100)
            .When(x => x.NameAr != null);


        RuleFor(x => x.NameEn)
            .MaximumLength(100)
            .When(x => x.NameEn != null);


        RuleFor(x => x.Code)
            .MaximumLength(50)
            .When(x => x.Code != null);


        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue);


        RuleFor(x => x.CalculationType)
            .IsInEnum()
            .When(x => x.CalculationType.HasValue);
    }
}