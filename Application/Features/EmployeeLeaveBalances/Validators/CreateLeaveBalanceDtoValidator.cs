using FluentValidation;
using MicroERP.Application.Features.EmployeeLeaveBalances.DTOs;

namespace MicroERP.Application.Features.EmployeeLeaveBalances.Validators;

public class CreateLeaveBalanceDtoValidator
    : AbstractValidator<CreateLeaveBalanceDto>
{
    public CreateLeaveBalanceDtoValidator()
    {
        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Year)
            .WithMessage("Invalid year");


        RuleFor(x => x.LeaveType)
            .IsInEnum()
            .WithMessage("Invalid leave type");


        RuleFor(x => x.TotalDays)
            .GreaterThan(0)
            .WithMessage("Total days must be greater than zero");
    }
}