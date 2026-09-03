using MicroERP.Domin.Common;

namespace MicroERP.Domin.Entities.Ticketing;

public class TicketCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<Ticket> Tickets { get; set; }
        = new List<Ticket>();
}