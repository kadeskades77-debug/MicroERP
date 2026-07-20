using FluentValidation;
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeSpecialLeaves.DTOs.Validators;

public class CreateSpecialLeaveDtoValidator
    : AbstractValidator<CreateSpecialLeaveDto>
{
    public CreateSpecialLeaveDtoValidator()
    {
        RuleFor(x => x.Type)
            .Must(BeValidSpecialLeaveType)
            .WithMessage(
                "Invalid special leave type. Allowed types are: BereavementFirstDegree, BereavementSecondDegree, Marriage.");
    }


    private bool BeValidSpecialLeaveType(
        SpecialLeaveType type)
    {
        return type == SpecialLeaveType.BereavementFirstDegree
            || type == SpecialLeaveType.BereavementSecondDegree
            || type == SpecialLeaveType.Marriage;
    }
}