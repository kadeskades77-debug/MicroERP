using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;

namespace MicroERP.Domin.Entities.Ticketing;

public class TicketComment : BaseEntity
{
    public int TicketId { get; set; }

    public int EmployeeId { get; set; }

    public string Comment { get; set; } = string.Empty;

    public Ticket Ticket { get; set; } = null!;

    public Employee Employee { get; set; } = null!;
}