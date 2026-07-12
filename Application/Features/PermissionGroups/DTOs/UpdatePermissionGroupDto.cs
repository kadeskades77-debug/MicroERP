namespace MicroERP.Application.Features.PermissionGroups.DTOs;

public class UpdatePermissionGroupDto
{
    public string Name { get; set; } = null!;

    public string Key { get; set; } = null!;

    public string? Description { get; set; }

}