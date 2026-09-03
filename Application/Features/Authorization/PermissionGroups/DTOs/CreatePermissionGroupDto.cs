namespace MicroERP.Application.Features.Authorization.PermissionGroups.DTOs;

public class CreatePermissionGroupDto
{
    public string Name { get; set; } = null!;

    public string Key { get; set; } = null!;

    public string? Description { get; set; }
}