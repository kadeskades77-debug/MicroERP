namespace MicroERP.Application.Features.Audit.DTOs;

public class AuditLogDto
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public string? UserName { get; set; }

    public string Action { get; set; } = null!;

    public string EntityName { get; set; } = null!;

    public string? EntityId { get; set; }

    public DateTime CreatedOn { get; set; }
}