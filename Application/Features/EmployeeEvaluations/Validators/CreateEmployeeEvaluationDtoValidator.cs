using FluentValidation;
namespace MicroERP.Application.Features.EmployeeEvaluations.Validators;

public class CreateEmployeeEvaluationDtoValidator
    : AbstractValidator<CreateEmployeeEvaluationDto>
{
    public CreateEmployeeEvaluationDtoValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .WithMessage("Employee is required.");


        RuleFor(x => x.PeriodId)
            .GreaterThan(0)
            .WithMessage("Evaluation period is required.");

        RuleFor(x => x.TemplateId)
            .GreaterThan(0)
            .WithMessage("Evaluation template is required.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Evaluation items are required.");

        RuleForEach(x => x.Items)
            .SetValidator(
                new CreateEmployeeEvaluationItemDtoValidator());
    }
}