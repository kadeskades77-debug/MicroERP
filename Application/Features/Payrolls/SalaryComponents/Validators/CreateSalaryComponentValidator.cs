using FluentValidation;
using MicroERP.Application.Features.Payrolls.SalaryComponents.DTOs;

namespace MicroERP.Application.Features.Payrolls.SalaryComponents.Validators;

public class CreateSalaryComponentValidator
    : AbstractValidator<CreateSalaryComponentDto>
{
    public CreateSalaryComponentValidator()
    {
        RuleFor(x => x.NameAr)
            .NotEmpty()
            .WithMessage("Arabic name is required")
            .MaximumLength(100);


        RuleFor(x => x.NameEn)
            .NotEmpty()
            .WithMessage("English name is required")
            .MaximumLength(100);


        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required")
            .MaximumLength(50);


        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid salary component type");


        RuleFor(x => x.CalculationType)
            .IsInEnum()
            .WithMessage("Invalid calculation type");
    }
}