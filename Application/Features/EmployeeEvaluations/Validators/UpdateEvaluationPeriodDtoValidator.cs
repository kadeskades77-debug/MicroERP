using FluentValidation;
namespace MicroERP.Application.Features.EmployeeEvaluations.Validators;

public sealed class UpdateEvaluationPeriodDtoValidator : AbstractValidator<UpdateEvaluationPeriodDto> { public UpdateEvaluationPeriodDtoValidator() { RuleFor(x=>x.Name).NotEmpty().MaximumLength(200); } }
