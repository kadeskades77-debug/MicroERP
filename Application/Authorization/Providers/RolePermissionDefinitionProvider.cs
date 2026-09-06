using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Authorization.Permissions;

namespace MicroERP.Application.Authorization.Providers;

public class RolePermissionDefinitionProvider
    : IMultiPermissionDefinitionProvider
{
    public IEnumerable<PermissionGroupDefinition> Groups =>
    [
        new(
            key: "Role",
            name: "Roles",
            description: "Role management permissions.",
            isSystem: true),

        new(
            key: "UserRole",
            name: "User Roles",
            description: "User role assignment permissions.",
            isSystem: true),

        new(
            key: "PermissionGroup",
            name: "Permission Groups",
            description: "Permission group management permissions.",
            isSystem: true),

        new(
            key: "UserPermissionGroup",
            name: "User Permission Groups",
            description: "User permission group assignment permissions.",
            isSystem: true),

        new(
            key: "Permission",
            name: "Permissions",
            description: "Permission management permissions.",
            isSystem: true)
    ];

    public IEnumerable<PermissionDefinition> GetPermissions()
    {
        // Role
        yield return new(
            "Role",
            RolePermissionPermissions.Role.View,
            "View Roles",
            "Allows viewing roles."
        );

        yield return new(
            "Role",
            RolePermissionPermissions.Role.Create,
            "Create Roles",
            "Allows creating roles."
        );

        yield return new(
            "Role",
            RolePermissionPermissions.Role.Update,
            "Update Roles",
            "Allows updating roles."
        );

        yield return new(
            "Role",
            RolePermissionPermissions.Role.Delete,
            "Delete Roles",
            "Allows deleting roles."
        );


        // UserRole
        yield return new(
            "UserRole",
            RolePermissionPermissions.UserRole.View,
            "View User Roles",
            "Allows viewing user role assignments."
        );

        yield return new(
            "UserRole",
            RolePermissionPermissions.UserRole.Replace,
            "Replace User Role",
            "Allows replacing the role assigned to a user."
        );

        yield return new(
            "UserRole",
            RolePermissionPermissions.UserRole.Delete,
            "Remove User Role",
            "Allows removing a role from a user."
        );

        yield return new(
            "UserRole",
            RolePermissionPermissions.UserRole.AssignUsers,
            "Assign Users",
            "Allows assigning users to roles."
        );

        // PermissionGroup
        yield return new(
            "PermissionGroup",
            RolePermissionPermissions.PermissionGroup.View,
            "View Permission Groups",
            "Allows viewing permission groups."
        );

        yield return new(
            "PermissionGroup",
            RolePermissionPermissions.PermissionGroup.Create,
            "Create Permission Groups",
            "Allows creating permission groups."
        );

        yield return new(
            "PermissionGroup",
            RolePermissionPermissions.PermissionGroup.Update,
            "Update Permission Groups",
            "Allows updating permission groups."
        );

        yield return new(
            "PermissionGroup",
            RolePermissionPermissions.PermissionGroup.Delete,
            "Delete Permission Groups",
            "Allows deleting permission groups."
        );

        // UserPermissionGroup
        yield return new(
            "UserPermissionGroup",
            RolePermissionPermissions.UserPermissionGroup.View,
            "View User Permission Groups",
            "Allows viewing user permission group assignments."
        );

        yield return new(
            "UserPermissionGroup",
            RolePermissionPermissions.UserPermissionGroup.Replace,
            "Replace User Permission Groups",
            "Allows replacing permission groups assigned to a user."
        );

        yield return new(
            "UserPermissionGroup",
            RolePermissionPermissions.UserPermissionGroup.Delete,
            "Remove User Permission Group",
            "Allows removing a permission group from a user."
        );

        yield return new(
            "UserPermissionGroup",
            RolePermissionPermissions.UserPermissionGroup.AssignUsers,
            "Assign Users to Permission Groups",
            "Allows assigning users to permission groups."
        );

        // Permission
        yield return new(
            "Permission",
            RolePermissionPermissions.Permission.View,
            "View Permissions",
            "Allows viewing permissions."
        );

        yield return new(
            "Permission",
            RolePermissionPermissions.Permission.Create,
            "Create Permissions",
            "Allows creating permissions."
        );

        yield return new(
            "Permission",
            RolePermissionPermissions.Permission.Update,
            "Update Permissions",
            "Allows updating permissions."
        );

        yield return new(
            "Permission",
            RolePermissionPermissions.Permission.Delete,
            "Delete Permissions",
            "Allows deleting permissions."
        );
    }
}