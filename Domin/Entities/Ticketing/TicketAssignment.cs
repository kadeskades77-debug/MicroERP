using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;

namespace MicroERP.Domin.Entities.Ticketing;

public class TicketAssignment : BaseEntity
{
    public int TicketId { get; set; }

    public int EmployeeId { get; set; }

    public int? DepartmentId { get; set; }

    public DateTime? UnassignedOn { get; set; }

    public bool IsCurrent { get; set; } = true;

    public Ticket Ticket { get; set; } = null!;

    public Employee Employee { get; set; } = null!;

    public Department? Department { get; set; }
}