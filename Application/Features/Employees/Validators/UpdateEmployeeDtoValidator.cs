using FluentValidation;
using MicroERP.Application.Features.Employees.DTOs;

namespace MicroERP.Application.Features.Employees.Validators
{
    public class UpdateEmployeeDtoValidator
        : AbstractValidator<UpdateEmployeeDto>
    {
        public UpdateEmployeeDtoValidator()
        {
            RuleFor(x => x.FullName)
                .MaximumLength(200);

            RuleFor(x => x.Phone)
                .MaximumLength(20);

            RuleFor(x => x.Email)
                .MaximumLength(256)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email));
        }
    }
}