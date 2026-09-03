using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Ticketing.DTOs;

public class ChangeTicketPriorityDto
{
    public int TicketId { get; set; }
    public TicketPriority Priority { get; set; }
    public string? Notes { get; set; }
}
