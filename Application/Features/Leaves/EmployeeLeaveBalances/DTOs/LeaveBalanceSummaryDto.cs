namespace MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.DTOs;

public class LeaveBalanceSummaryDto
{
    public int TotalEmployees { get; set; }


    public int TotalBalanceDays { get; set; }


    public int UsedDays { get; set; }


    public int RemainingDays { get; set; }
}