using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Authorization.Permissions;

namespace MicroERP.Application.Authorization.Providers;

public class EmployeeDefinitionProvider
    : IPermissionDefinitionProvider
{
    public PermissionGroupDefinition Group =>
        new(
            key: "Employee",
            name: "Employee",
            description: "Basic employee permissions.",
            isSystem: true
        );


    public IEnumerable<PermissionDefinition> GetPermissions()
    {
        return
        [
            new(
                group: Group.Key,
                key: EmployeePermissions.Profile.View,
                name: "View Profile",
                description: "Allows employee to view own profile."
            ),

            new(
                group: Group.Key,
                key: EmployeePermissions.Profile.ChangePassword,
                name: "Change Password",
                description: "Allows employee to change own password."
            )
        ];
    }
}