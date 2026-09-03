using FluentValidation;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Validators.Overtime;

public class OvertimeApprovalDtoValidator
    : AbstractValidator<OvertimeApprovalDto>
{
    public OvertimeApprovalDtoValidator()
    {
        RuleFor(x => x.RejectionReason)
            .MaximumLength(1000);

        When(x => !x.Approve, () =>
        {
            RuleFor(x => x.RejectionReason)
                .NotEmpty()
                .WithMessage(
                    "Rejection reason is required.");
        });
    }
}