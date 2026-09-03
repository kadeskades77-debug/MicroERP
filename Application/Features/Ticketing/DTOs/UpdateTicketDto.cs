using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Ticketing.DTOs;

public class UpdateTicketDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public TicketPriority? Priority { get; set; }
    public DateTime? DueOn { get; set; }
}
