using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.DTOs;

public class LeaveBalanceReportDto
{
    public int EmployeeId { get; set; }


    public string EmployeeName { get; set; } = null!;


    public string Department { get; set; } = null!;



    public LeaveType LeaveType { get; set; }



    public int Year { get; set; }



    public int TotalDays { get; set; }



    public int UsedDays { get; set; }



    public int RemainingDays { get; set; }



    public double UsagePercentage { get; set; }
}