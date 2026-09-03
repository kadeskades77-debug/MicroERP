namespace MicroERP.Application.Features.Authorization.Permissions.DTOs;
public class CreatePermissionDto
{
    public string Key { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}