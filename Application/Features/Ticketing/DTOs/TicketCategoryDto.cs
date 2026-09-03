namespace MicroERP.Application.Features.Ticketing.DTOs;

public class TicketCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
