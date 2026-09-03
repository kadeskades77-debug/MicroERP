using FluentValidation;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Validators.Overtime;

public class CreateEmployeeOvertimeDtoValidator
    : AbstractValidator<CreateManualOvertimeDto>
{
    public CreateEmployeeOvertimeDtoValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0);


        RuleFor(x => x.StartDateTime)
            .NotEmpty();

        RuleFor(x => x.EndDateTime)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => x.EndDateTime > x.StartDateTime)
            .WithMessage(
                "End time must be greater than start time.");

        RuleFor(x => x.Reason)
            .MaximumLength(1000);
    }
}