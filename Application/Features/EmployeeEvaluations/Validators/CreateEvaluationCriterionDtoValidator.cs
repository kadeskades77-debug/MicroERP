using FluentValidation;
namespace MicroERP.Application.Features.EmployeeEvaluations.Validators;

public class CreateEvaluationCriterionDtoValidator
    : AbstractValidator<CreateEvaluationCriterionDto>
{
    public CreateEvaluationCriterionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Criterion name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.MaxScore)
            .GreaterThan(0)
            .WithMessage("Maximum score must be greater than zero.");

        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than zero.")
            .LessThanOrEqualTo(100)
            .WithMessage("Weight cannot be greater than 100.");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Sort order cannot be negative.");
    }
}