using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Ticketing.DTOs;

public class CreateTicketDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int CategoryId { get; set; }
    public TicketPriority Priority { get; set; }
    public int? AssignedToEmployeeId { get; set; }
    public int? AssignedDepartmentId { get; set; }
    public DateTime? DueOn { get; set; }
}
