using FluentValidation;
using MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaves.Validators;

public class RejectLeaveDtoValidator : AbstractValidator<RejectLeaveDto>
{
    public RejectLeaveDtoValidator()
    {
        RuleFor(x => x.RejectionReason)
            .NotEmpty()
            .WithMessage("Rejection reason is required")

            .MaximumLength(500)
            .WithMessage("Rejection reason cannot exceed 500 characters");
    }
}