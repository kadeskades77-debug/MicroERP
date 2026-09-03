using FluentValidation;
namespace MicroERP.Application.Features.EmployeeEvaluations.Validators;

public class CreateEvaluationPeriodDtoValidator
    : AbstractValidator<CreateEvaluationPeriodDto>
{
    public CreateEvaluationPeriodDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Evaluation period name is required.")
            .MaximumLength(200);

     }
}