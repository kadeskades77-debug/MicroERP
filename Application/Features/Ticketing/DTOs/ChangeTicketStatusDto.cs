using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Ticketing.DTOs;

public class ChangeTicketStatusDto
{
    public int TicketId { get; set; }
    public TicketStatus Status { get; set; }
    public string? Notes { get; set; }
}
