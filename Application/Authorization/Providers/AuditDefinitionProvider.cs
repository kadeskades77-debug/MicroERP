using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Authorization.Permissions;

namespace MicroERP.Application.Authorization.Providers;

public class AuditDefinitionProvider : IPermissionDefinitionProvider
{
    public PermissionGroupDefinition Group =>
        new(
            key: "Audit",
            name: "Audit",
            description: "Audit Logs",
            isSystem: true);

    public IEnumerable<PermissionDefinition> GetPermissions()
    {
        return
        [
            new(
                AuditPermissions.Audit.View,
                "View Audit Logs",
                "Allows viewing audit logs.")
        ];
    }
}