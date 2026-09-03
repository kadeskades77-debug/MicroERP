namespace MicroERP.Application.Features.Ticketing.DTOs;

public class AddTicketCommentDto
{
    public int TicketId { get; set; }
    public string Comment { get; set; } = null!;
}
