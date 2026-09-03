using FluentValidation;
using MicroERP.Application.Features.Payrolls.EmployeeLoans.DTOs;

namespace MicroERP.Application.Features.Payrolls.EmployeeLoans.Validators;

public class UpdateEmployeeLoanValidator : AbstractValidator<UpdateEmployeeLoanDto>
{
    public UpdateEmployeeLoanValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(200)
            .When(x => x.Title != null);

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .When(x => x.Notes != null);

    }
}
