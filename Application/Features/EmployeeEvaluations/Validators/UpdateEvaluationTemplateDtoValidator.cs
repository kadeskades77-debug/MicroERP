using FluentValidation;
namespace MicroERP.Application.Features.EmployeeEvaluations.Validators;

public class UpdateEvaluationTemplateDtoValidator
    : AbstractValidator<UpdateEvaluationTemplateDto>
{
    public UpdateEvaluationTemplateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Template name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.Criteria)
            .NotEmpty()
            .WithMessage("At least one evaluation criterion is required.");

        RuleForEach(x => x.Criteria)
            .SetValidator(
                new UpdateEvaluationCriterionDtoValidator());
    }
}