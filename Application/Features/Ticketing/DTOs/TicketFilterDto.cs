namespace MicroERP.Application.Features.Ticketing.DTOs;

public class TicketFilterDto
{
    public string? Search { get; set; }
    public int? Status { get; set; }
    public int? Priority { get; set; }
    public int? CategoryId { get; set; }
    public int? AssignedToEmployeeId { get; set; }
    public int? AssignedDepartmentId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
