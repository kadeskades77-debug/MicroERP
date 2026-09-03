using FluentValidation;
using MicroERP.Application.Features.Payrolls.Adjustments.DTOs;

namespace MicroERP.Application.Features.Payrolls.Adjustments.Validators;

public class CreatePayrollAdjustmentValidator
    : AbstractValidator<CreatePayrollAdjustmentDto>
{
    public CreatePayrollAdjustmentValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .WithMessage("Employee is required.");


        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid adjustment type.");


        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")

            .MaximumLength(200)
            .WithMessage("Title maximum length is 200 characters.");


        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero.");
    }
}