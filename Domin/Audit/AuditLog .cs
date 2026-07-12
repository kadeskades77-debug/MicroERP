using Domin.Entities;
using MicroERP.Domin.Common;

namespace MicroERP.Domain.Audit;

public class AuditLog : BaseEntity
{
    public string? UserId { get; set; }

    public ApplicationUser? User { get; set; }

    public string? UserName { get; set; }

    public string Action { get; set; } = null!;

    public string EntityName { get; set; } = null!;

    public string? EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }
}