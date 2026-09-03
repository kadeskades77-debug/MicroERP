namespace MicroERP.Application.Features.Authentication.Roles.DTOs;

public class RoleDto
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsSystem { get; set; }
    public List<string> PermissionGroupKeys { get; set; } = [];
}