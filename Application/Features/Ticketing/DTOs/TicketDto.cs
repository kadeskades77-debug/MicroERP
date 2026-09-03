using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Ticketing.DTOs;

public class TicketDto
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; } 
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? CreatedByName { get; set; }
    public int? AssignedToEmployeeId { get; set; }
    public string? AssignedToEmployeeName { get; set; }
    public int? AssignedDepartmentId { get; set; }
    public string? AssignedDepartmentName { get; set; }
    public DateTime? DueOn { get; set; }
    public DateTime? ResolvedOn { get; set; }
    public DateTime? ClosedOn { get; set; }
    public DateTime CreatedOn { get; set; }
}
