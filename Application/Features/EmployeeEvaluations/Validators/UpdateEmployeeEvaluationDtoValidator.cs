using FluentValidation;
namespace MicroERP.Application.Features.EmployeeEvaluations.Validators;

public class UpdateEmployeeEvaluationDtoValidator
    : AbstractValidator<UpdateEmployeeEvaluationDto>
{
    public UpdateEmployeeEvaluationDtoValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Evaluation items are required.");

        RuleForEach(x => x.Items)
            .SetValidator(
                new UpdateEmployeeEvaluationItemDtoValidator());
    }
}