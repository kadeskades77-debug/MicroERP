using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.Ticketing;

public class TicketHistory : BaseEntity
{
    public int TicketId { get; set; }

    public TicketHistoryAction Action { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? Notes { get; set; }

    public Ticket? Ticket { get; set; }
}