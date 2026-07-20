using MicroERP.Application.Features.Permissions.DTOs;
using MicroERP.Domain.Identity;

namespace MicroERP.Application.Common.Mappings;

public static class PermissionMappings
{
    public static PermissionDto ToDto(this Permission permission)
    {
        return new PermissionDto
        {
            Id = permission.Id,

            Key = permission.Key,

            Name = permission.Name,

            Description = permission.Description
        };
    }
}