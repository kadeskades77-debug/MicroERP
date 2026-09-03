using FluentValidation;
using MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaves.Validators;

public class CreateLeaveDtoValidator : AbstractValidator<CreateLeaveDto>
{
    public CreateLeaveDtoValidator()
    {
        RuleFor(x => x.LeaveType)
            .IsInEnum()
            .WithMessage("Invalid leave type");


        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required");


        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required");


        RuleFor(x => x)
            .Must(x => x.StartDate <= x.EndDate)
            .WithMessage("Start date cannot be after end date");


        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");
    }
}