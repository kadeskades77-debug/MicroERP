using FluentValidation;
using MicroERP.Application.Features.Departments.DTOs;

namespace MicroERP.Application.Features.Departments.Validators
{
    public class AssignDepartmentManagerValidator
        : AbstractValidator<AssignDepartmentManagerDto>
    {
        public AssignDepartmentManagerValidator()
        {
        }
    }
}