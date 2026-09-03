using FluentValidation;
namespace MicroERP.Application.Features.EmployeeEvaluations.Validators;

public class UpdateEmployeeEvaluationItemDtoValidator
    : AbstractValidator<UpdateEmployeeEvaluationItemDto>
{
    public UpdateEmployeeEvaluationItemDtoValidator()
    {
        RuleFor(x => x.CriterionId)
            .GreaterThan(0)
            .WithMessage("Criterion is required.");

        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Score cannot be negative.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000);
    }
}