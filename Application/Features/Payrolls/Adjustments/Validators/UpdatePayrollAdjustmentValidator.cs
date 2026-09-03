using FluentValidation;
using MicroERP.Application.Features.Payrolls.Adjustments.DTOs;

namespace MicroERP.Application.Features.Payrolls.Adjustments.Validators;

public class UpdatePayrollAdjustmentValidator
    : AbstractValidator<UpdatePayrollAdjustmentDto>
{
    public UpdatePayrollAdjustmentValidator()
    {



        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue)
            .WithMessage("Invalid adjustment type.");


        RuleFor(x => x.Title)
            .NotEmpty()
            .When(x => x.Title != null)
            .WithMessage("Title cannot be empty.")

            .MaximumLength(200)
            .When(x => x.Title != null)
            .WithMessage("Title maximum length is 200 characters.");


        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .When(x => x.Amount.HasValue)
            .WithMessage("Amount must be greater than zero.");
    }
}