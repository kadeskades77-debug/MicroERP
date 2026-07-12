namespace MicroERP.Application.Features.Roles.DTOs;

public class CreateRoleDto
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}