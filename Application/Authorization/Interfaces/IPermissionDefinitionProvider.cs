namespace MicroERP.Application.Authorization.Interfaces;

public interface IPermissionDefinitionProvider
{
     PermissionGroupDefinition Group { get; }

    IEnumerable<PermissionDefinition> GetPermissions();
}