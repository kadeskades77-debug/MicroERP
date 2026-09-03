using FluentValidation;
namespace MicroERP.Application.Features.EmployeeEvaluations.Validators;

public sealed class UpdateEvaluationCriterionDtoValidator : AbstractValidator<UpdateEvaluationCriterionDto> { public UpdateEvaluationCriterionDtoValidator() { RuleFor(x=>x.Id).GreaterThan(0); RuleFor(x=>x.Name).NotEmpty().MaximumLength(200); RuleFor(x=>x.MaxScore).GreaterThan(0).LessThanOrEqualTo(100); RuleFor(x=>x.Weight).GreaterThan(0).LessThanOrEqualTo(100); RuleFor(x=>x.SortOrder).GreaterThan(0); } }
