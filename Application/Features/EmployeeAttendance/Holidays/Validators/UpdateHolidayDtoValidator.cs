using FluentValidation;
using MicroERP.Application.Features.EmployeeAttendance.Holidays.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Holidays.Validators;

public class UpdateHolidayDtoValidator
    : AbstractValidator<UpdateHolidayDto>
{
    public UpdateHolidayDtoValidator()
    {
        When(x => x.Name != null, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);
        });

        When(x => x.StartDate.HasValue && x.EndDate.HasValue, () =>
        {
            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate!.Value)
                .WithMessage("End date must be greater than or equal to start date.");
        });

        When(x => x.Type.HasValue, () =>
        {
            RuleFor(x => x.Type)
                .IsInEnum();
        });

        When(x => x.Notes != null, () =>
        {
            RuleFor(x => x.Notes)
                .MaximumLength(1000);
        });
    }
}