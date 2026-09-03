using FluentValidation;
using MicroERP.Application.Features.Payrolls.EmployeeLoans.DTOs;

namespace MicroERP.Application.Features.Payrolls.EmployeeLoans.Validators;

public class CreateEmployeeLoanValidator : AbstractValidator<CreateEmployeeLoanDto>
{
    public CreateEmployeeLoanValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .WithMessage("Employee is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0)
            .WithMessage("Total amount must be greater than zero.");

        RuleFor(x => x.InstallmentAmount)
            .GreaterThan(0)
            .WithMessage("Installment amount must be greater than zero.");


        RuleFor(x => x)
            .Must(x => x.InstallmentAmount <= x.TotalAmount)
            .WithMessage("Installment amount cannot exceed total amount.");

        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.Notes)
            .MaximumLength(1000);
    }
}
