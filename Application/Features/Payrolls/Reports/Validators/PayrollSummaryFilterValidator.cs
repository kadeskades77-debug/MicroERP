using FluentValidation;
using MicroERP.Application.Features.Payrolls.Reports.DTOs;

namespace MicroERP.Application.Features.Payrolls.Reports.Validators;

public class PayrollSummaryFilterValidator
    : AbstractValidator<PayrollSummaryFilterDto>
{
    public PayrollSummaryFilterValidator()
    {
        RuleFor(x => x.PayrollPeriodId)
            .GreaterThan(0);
    }
}