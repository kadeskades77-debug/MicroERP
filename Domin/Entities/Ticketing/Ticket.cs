using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.Ticketing;

public class Ticket : BaseEntity
{
    public string TicketNumber { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TicketStatus Status { get; set; } = TicketStatus.Open;

    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    public int CategoryId { get; set; }

    public int? AssignedToEmployeeId { get; set; }

    public int? AssignedDepartmentId { get; set; }

    public DateTime? ResolvedOn { get; set; }

    public DateTime? ClosedOn { get; set; }

    public DateTime? DueOn { get; set; }

    // Navigation Properties

    public TicketCategory Category { get; set; } = null!;

    public Employee? AssignedToEmployee { get; set; }

    public Department? AssignedDepartment { get; set; }

    public ICollection<TicketComment> Comments { get; set; }
        = new List<TicketComment>();

    public ICollection<TicketAssignment> Assignments { get; set; }
        = new List<TicketAssignment>();

    public ICollection<TicketAttachment> Attachments { get; set; }
        = new List<TicketAttachment>();

    public ICollection<TicketHistory> History { get; set; }
        = new List<TicketHistory>();
}