using MicroERP.Domin.Identity;

namespace MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.DTOs;

public class SpecialLeaveDto
{
    public int Id { get; set; }


    public int EmployeeId { get; set; }


    public string EmployeeName { get; set; } = null!;


    public int TypeId { get; set; }


    public string Type { get; set; } = null!;


    public DateOnly StartDate { get; set; }


    public DateOnly EndDate { get; set; }


    public int TotalDays { get; set; }


    public int StatusId { get; set; }


    public string Status { get; set; } = null!;


    public string? Reason { get; set; }


    public string? AttachmentPath { get; set; }


    public string? RejectionReason { get; set; }
    
    public string? ApprovedByUserName { get; set; }
    public ApplicationUser? ApprovedByUser { get; set; }
    public string? RejectedByUserName { get; set; }
    public ApplicationUser? RejectedByUser { get; set; }
    public DateTime? ApprovedOn { get; set; }


    public DateTime? RejectedOn { get; set; }
}