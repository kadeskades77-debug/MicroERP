

namespace MicroERP.Application.Authorization.Interfaces;

public interface IPermissionDefinitionService
{
    IEnumerable<PermissionGroupDefinition> GetGroups();

    IEnumerable<PermissionDefinition> GetPermissions();

    PermissionGroupDefinition? GetGroup(string key);

    PermissionDefinition? GetPermission(string name);
}