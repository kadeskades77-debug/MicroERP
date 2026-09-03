namespace MicroERP.Application.Features.Ticketing.DTOs;

public class AssignTicketDto
{
    public int TicketId { get; set; }
    public int EmployeeId { get; set; }
    public string? Notes { get; set; }
}
