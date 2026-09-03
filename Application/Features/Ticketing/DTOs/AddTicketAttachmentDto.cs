using Microsoft.AspNetCore.Http;

namespace MicroERP.Application.Features.Ticketing.DTOs;

public class AddTicketAttachmentDto
{
    public int TicketId { get; set; }
    public IFormFile File { get; set; } = null!;
}
