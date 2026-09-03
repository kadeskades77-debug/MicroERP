

namespace MicroERP.Application.Features.Payrolls.Reports.Validators
{
    using FluentValidation;
    using MicroERP.Application.Features.Payrolls.Reports.DTOs;

    public class EmployeePayrollHistoryFilterValidator
        : AbstractValidator<EmployeePayrollHistoryFilterDto>
    {
        public EmployeePayrollHistoryFilterValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0);
        }
    }
}
