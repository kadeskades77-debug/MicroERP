using FluentValidation;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Validators;

public class CreateAttendanceCorrectionValidator
    : AbstractValidator<CreateAttendanceCorrectionDto>
{
    public CreateAttendanceCorrectionValidator()
    {
        RuleFor(x => x.AttendanceRecordId)
            .GreaterThan(0);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x)
            .Must(HaveAnyChange)
            .WithMessage(
                "At least one value must be changed.");
    }

    private static bool HaveAnyChange(
        CreateAttendanceCorrectionDto dto)
    {
        return dto.NewCheckIn.HasValue ||
               dto.NewCheckOut.HasValue ||
               dto.NewStatus.HasValue;
    }
}