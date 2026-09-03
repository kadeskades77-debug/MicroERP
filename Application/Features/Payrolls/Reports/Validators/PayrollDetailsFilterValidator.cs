
using FluentValidation;

namespace MicroERP.Application.Features.Payrolls.Reports.Validators
{
    using MicroERP.Application.Features.Payrolls.Reports.DTOs;

    public class PayrollDetailsFilterValidator
        : AbstractValidator<PayrollDetailsFilterDto>
    {
        public PayrollDetailsFilterValidator()
        {
            RuleFor(x => x.PayrollId)
                .GreaterThan(0);
        }
    }
}
