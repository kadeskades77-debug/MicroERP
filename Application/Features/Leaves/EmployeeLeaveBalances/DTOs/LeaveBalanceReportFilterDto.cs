using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.DTOs;

public class LeaveBalanceReportFilterDto
{
    public int? EmployeeId { get; set; }

    public int? DepartmentId { get; set; }

    public LeaveType? LeaveType { get; set; }

    public int? Year { get; set; }
}