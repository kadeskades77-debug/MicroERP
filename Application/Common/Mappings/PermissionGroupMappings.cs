using MicroERP.Application.Features.Authorization.PermissionGroups.DTOs;
using MicroERP.Domain.Identity;

namespace MicroERP.Application.Common.Mappings;

public static class PermissionGroupMappings
{
    public static PermissionGroupDto ToDto(
        this PermissionGroup group)
    {
        return new PermissionGroupDto
        {
            Id = group.Id,

            Name = group.Name,

            Key = group.Key,

            Description = group.Description,

            IsSystem = group.IsSystem,

            Permissions = group.PermissionGroupPermissions
                .Select(x => x.Permission.Key)
                .ToList()
        };
    }
}