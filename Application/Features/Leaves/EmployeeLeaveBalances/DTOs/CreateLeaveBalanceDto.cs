using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.DTOs;

public class CreateLeaveBalanceDto
{
    public int Year { get; set; }

    public LeaveType LeaveType { get; set; }

    public int TotalDays { get; set; }
}