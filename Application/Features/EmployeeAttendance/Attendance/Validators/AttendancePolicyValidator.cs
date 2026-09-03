using FluentValidation;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Validators;

public class AttendancePolicyValidator
    : AbstractValidator<CreateAttendancePolicyDto>
{
    public AttendancePolicyValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);


        RuleFor(x => x.AbsentPenaltyPoints)
            .GreaterThanOrEqualTo(0);


        RuleFor(x => x.MissingCheckInPenaltyPoints)
            .GreaterThanOrEqualTo(0);


        RuleFor(x => x.MissingCheckOutPenaltyPoints)
            .GreaterThanOrEqualTo(0);


        RuleFor(x => x.LateMinutesPerPenaltyPoint)
            .GreaterThan(0);

        RuleFor(x => x.LostMinutesPerPenaltyPoint)
            .GreaterThan(0);


        RuleFor(x => x.MinimumPerformanceScore)
            .InclusiveBetween(0, 100);
    }
}