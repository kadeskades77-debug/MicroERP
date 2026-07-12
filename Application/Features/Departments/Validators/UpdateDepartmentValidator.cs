using FluentValidation;
using MicroERP.Application.Features.Departments.DTOs;

namespace MicroERP.Application.Features.Departments.Validators
{
    public class UpdateDepartmentValidator
        : AbstractValidator<UpdateDepartmentDto>
    {
        public UpdateDepartmentValidator()
        {
            RuleFor(x => x.NameAr)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.NameEn)
                .NotEmpty()
                .MaximumLength(200);
        }
    }
}