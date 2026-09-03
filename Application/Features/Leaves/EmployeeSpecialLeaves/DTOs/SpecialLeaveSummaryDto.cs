namespace MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.DTOs;

public class SpecialLeaveSummaryDto
{
    public int TotalRequests { get; set; }

    public int Approved { get; set; }

    public int Pending { get; set; }

    public int Rejected { get; set; }

    public int Cancelled { get; set; }


    public int TotalDays { get; set; }
}