using FluentValidation;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Validators.Overtime
{
    public class UpdateOvertimeValidator
      : AbstractValidator<UpdateOvertimeDto>
    {
        public UpdateOvertimeValidator()
        {
            RuleFor(x => x)
    .Must(x => x.EndDateTime > x.StartDateTime)
    .WithMessage(
        "End time must be greater than start time.");

            RuleFor(x => x.HourlyRate)
                .GreaterThan(0)
                .LessThanOrEqualTo(1_000_000)
                .When(x => x.HourlyRate.HasValue);

            RuleFor(x => x.Multiplier)
                .GreaterThan(0)
                .LessThanOrEqualTo(10)
                .When(x => x.Multiplier.HasValue);

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100_000_000)
                .When(x => x.Amount.HasValue);
        }
    }
}
