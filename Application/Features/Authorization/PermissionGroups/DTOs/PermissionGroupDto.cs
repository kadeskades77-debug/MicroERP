namespace MicroERP.Application.Features.Authorization.PermissionGroups.DTOs;

public class PermissionGroupDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Key { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsSystem { get; set; }

    public List<string> Permissions { get; set; } = [];
}