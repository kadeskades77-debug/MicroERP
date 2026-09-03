namespace MicroERP.Application.Features.Ticketing.DTOs;

public class TicketCommentDto { 
    public int Id { get; set; }
    public int TicketId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string Comment { get; set; } = string.Empty; 
    public DateTime CreatedOn { get; set; } 
    public DateTime? ModifiedOn { get; set; } 
}
