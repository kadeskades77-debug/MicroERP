using FluentValidation;
using MicroERP.Application.Features.Employees.DTOs;
namespace MicroERP.Application.Features.Employees.Validators
{
    public class CreateEmployeeDtoValidator
      : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .MaximumLength(200);

            RuleFor(x => x.Phone)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.Salary)
                .GreaterThan(0);

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0);

            When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
            {
                RuleFor(x => x.Email)
                    .MaximumLength(256)
                    .EmailAddress();
            });
        }
    }
}
