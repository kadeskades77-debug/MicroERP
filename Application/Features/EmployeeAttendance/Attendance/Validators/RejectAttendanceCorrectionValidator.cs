using FluentValidation;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Validators;

public class RejectAttendanceCorrectionValidator
    : AbstractValidator<RejectAttendanceCorrectionDto>
{
    public RejectAttendanceCorrectionValidator()
    {
        RuleFor(x => x.CorrectionId)
            .GreaterThan(0);

        RuleFor(x => x.RejectionReason)
            .NotEmpty()
            .MaximumLength(500);
    }
}