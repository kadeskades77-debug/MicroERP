using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.DTOs;

public class SpecialLeaveReportDto
{
    public int LeaveId { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public string Department { get; set; } = null!;


    public SpecialLeaveType Type { get; set; }


    public DateOnly StartDate { get; set; }


    public DateOnly EndDate { get; set; }


    public int TotalDays { get; set; }


    public SpecialLeaveStatus Status { get; set; }


    public string? Reason { get; set; }


    public DateTime? ApprovedOn { get; set; }
}