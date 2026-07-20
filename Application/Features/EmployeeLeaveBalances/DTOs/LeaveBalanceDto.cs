namespace MicroERP.Application.Features.EmployeeLeaveBalances.DTOs;

public class LeaveBalanceDto
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public int Year { get; set; }


    public int LeaveTypeId { get; set; }

    public string LeaveType { get; set; } = null!;


    public int TotalDays { get; set; }

    public int UsedDays { get; set; }

    public int RemainingDays { get; set; }
}