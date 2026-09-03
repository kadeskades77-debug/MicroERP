using FluentValidation;
using MicroERP.Application.Features.EmployeeAttendance.Holidays.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Holidays.Validators;

public class CreateHolidayDtoValidator
    : AbstractValidator<CreateHolidayDto>
{
    public CreateHolidayDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.StartDate)
            .NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be greater than or equal to start date.");

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}