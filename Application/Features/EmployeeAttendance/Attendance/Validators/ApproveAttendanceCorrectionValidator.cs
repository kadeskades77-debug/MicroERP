using FluentValidation;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Validators;

public class ApproveAttendanceCorrectionValidator
    : AbstractValidator<ApproveAttendanceCorrectionDto>
{
    public ApproveAttendanceCorrectionValidator()
    {
        RuleFor(x => x.CorrectionId)
            .GreaterThan(0);
    }
}