using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeLeaveBalances.DTOs;

public class CreateLeaveBalanceDto
{
    public int Year { get; set; }

    public LeaveType LeaveType { get; set; }

    public int TotalDays { get; set; }
}