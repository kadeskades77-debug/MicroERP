namespace MicroERP.Application.Features.Ticketing.DTOs;

public class TicketAttachmentDto
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }
    public DateTime CreatedOn { get; set; }
}
