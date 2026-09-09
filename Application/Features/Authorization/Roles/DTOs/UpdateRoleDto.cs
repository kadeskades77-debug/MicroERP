namespace MicroERP.Application.Features.Authorization.Roles.DTOs;

public class UpdateRoleDto
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}