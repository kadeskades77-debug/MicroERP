using FluentValidation;
using MicroERP.Application.Features.Departments.DTOs;

namespace MicroERP.Application.Features.Departments.Validators
{
    public class CreateDepartmentValidator
        : AbstractValidator<CreateDepartmentDto>
    {
        public CreateDepartmentValidator()
        {
            RuleFor(x => x.NameAr)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.NameEn)
                .NotEmpty()
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.NameEn));
        }
    }
}