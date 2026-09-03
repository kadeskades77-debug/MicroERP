using FluentValidation;
using MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaves.Validators;

public class UpdateLeaveDtoValidator : AbstractValidator<UpdateLeaveDto>
{
    public UpdateLeaveDtoValidator()
    {
        RuleFor(x => x.LeaveType)
            .IsInEnum()
            .When(x => x.LeaveType.HasValue)
            .WithMessage("Invalid leave type");


        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => x.Reason != null)
            .WithMessage("Reason cannot exceed 500 characters");


        RuleFor(x => x)
            .Must(x =>
                !x.StartDate.HasValue ||
                !x.EndDate.HasValue ||
                x.StartDate <= x.EndDate)
            .WithMessage("Start date cannot be after end date");
    }
}