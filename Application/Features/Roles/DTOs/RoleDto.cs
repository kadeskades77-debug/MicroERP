namespace MicroERP.Application.Features.Roles.DTOs;

public class RoleDto
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsSystem { get; set; }
    public List<string> PermissionGroupKeys { get; set; } = [];
}