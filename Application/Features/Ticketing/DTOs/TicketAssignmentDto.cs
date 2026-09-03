namespace MicroERP.Application.Features.Ticketing.DTOs;

public class TicketAssignmentDto
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UnassignedOn { get; set; }
}
