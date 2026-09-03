using FluentValidation;
using MicroERP.Application.Features.Payrolls.DTOs;

namespace MicroERP.Application.Features.Payrolls.Validators;

public class CreatePayrollPeriodValidator
    : AbstractValidator<CreatePayrollPeriodDto>
{
    public CreatePayrollPeriodValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100);


        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12);
    }
}