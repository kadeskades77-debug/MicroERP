namespace MicroERP.Application.Features.EmployeeLeaveBalances.DTOs;

public class LeavePolicyDto
{
    public int Id { get; set; }


    public int LeaveTypeId { get; set; }

    public string LeaveType { get; set; } = null!;


    public decimal DaysPerMonth { get; set; }


    public int MaximumDaysPerYear { get; set; }
}