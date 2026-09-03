

namespace MicroERP.Application.Authorization.Interfaces
{
    public interface IMultiPermissionDefinitionProvider
    {
        IEnumerable<PermissionGroupDefinition> Groups { get; }

        IEnumerable<PermissionDefinition> GetPermissions();
    }
}
