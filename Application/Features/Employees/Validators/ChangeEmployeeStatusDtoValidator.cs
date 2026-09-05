

using FluentValidation;
using MicroERP.Application.Features.Employees.DTOs;

namespace MicroERP.Application.Features.Employees.Validators
{
 
public class ChangeEmployeeStatusDtoValidator
    : AbstractValidator<ChangeEmployeeStatusDto>
    {
        public ChangeEmployeeStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid employee status.");
        }
    }


}
