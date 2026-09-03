using FluentValidation;
using MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.Validators;

public class UpdateWorkScheduleDtoValidator
    : AbstractValidator<UpdateWorkScheduleDto>
{
    public UpdateWorkScheduleDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100);



        RuleFor(x => x.FirstShiftEnd)
            .Must((dto, end) =>
            {
                if (!dto.FirstShiftStart.HasValue ||
                    !end.HasValue)
                    return true;

                return end.Value >
                       dto.FirstShiftStart.Value;
            })
            .WithMessage(
                "First shift end must be later than start.");



        RuleFor(x => x.SecondShiftEnd)
            .Must((dto, end) =>
            {
                if (!dto.SecondShiftStart.HasValue ||
                    !end.HasValue)
                    return true;

                return end.Value >
                       dto.SecondShiftStart.Value;
            })
            .WithMessage(
                "Second shift end must be later than start.");



        RuleFor(x => x.LateGraceMinutes)
            .GreaterThanOrEqualTo(0);



        RuleFor(x => x.EarlyLeaveGraceMinutes)
            .GreaterThanOrEqualTo(0);



        RuleFor(x => x.MinimumWorkMinutes)
            .GreaterThan(0);
    }
}