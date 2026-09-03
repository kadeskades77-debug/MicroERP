using FluentValidation;
using MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.Validators;

public class CreateWorkScheduleDtoValidator
    : AbstractValidator<CreateWorkScheduleDto>
{
    public CreateWorkScheduleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);



        RuleFor(x => x.FirstShiftStart)
            .NotEmpty();


        RuleFor(x => x.FirstShiftEnd)
            .NotEmpty()
            .Must((dto, end) =>
                end > dto.FirstShiftStart)
            .WithMessage(
                "First shift end must be later than start.");



        RuleFor(x => x.SecondShiftEnd)
            .Must((dto, end) =>
            {
                if (!dto.SecondShiftStart.HasValue &&
                    !end.HasValue)
                    return true;

                if (dto.SecondShiftStart.HasValue &&
                    end.HasValue)
                    return end.Value >
                           dto.SecondShiftStart.Value;

                return false;
            })
            .WithMessage(
                "Second shift times must be valid.");



        RuleFor(x => x.LateGraceMinutes)
            .GreaterThanOrEqualTo(0);


        RuleFor(x => x.EarlyLeaveGraceMinutes)
            .GreaterThanOrEqualTo(0);



        RuleFor(x => x.MinimumWorkMinutes)
            .GreaterThan(0);
    }
}