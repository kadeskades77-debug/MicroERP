namespace MicroERP.Application.Features.Ticketing.DTOs;

public class TicketListDto
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string? AssignedToEmployeeName { get; set; }
    public string? AssignedDepartmentName { get; set; }
    public DateTime? DueOn { get; set; }
    public DateTime CreatedOn { get; set; }
}
