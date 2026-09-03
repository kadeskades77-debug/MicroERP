namespace MicroERP.Application.Features.Authorization.Permissions.DTOs;

public class UpdatePermissionDto
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}