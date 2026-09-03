using MicroERP.Domin.Common;

namespace MicroERP.Domin.Entities.Ticketing;

public class TicketAttachment : BaseEntity
{
    public int TicketId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public Ticket Ticket { get; set; } = null!;
}