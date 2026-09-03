namespace MicroERP.Application.Features.Ticketing.DTOs;

public class TicketHistoryDto
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string Action { get; set; } = null!;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Notes { get; set; }
    public string? PerformedBy { get; set; }
    public DateTime CreatedOn { get; set; }
}
