using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Authorization.Permissions;

namespace MicroERP.Application.Authorization.Providers;

public class RolePermissionDefinitionProvider : IPermissionDefinitionProvider
{
    public PermissionGroupDefinition Group =>
        new(
            key: "RolePermission",
            name: "Role & Permission Management",
            description: "Role and Permission Management Module",
            isSystem: true
        );

    public IEnumerable<PermissionDefinition> GetPermissions()
    {
        return
        [
            //================ UserRole =================

          new(
              Group.Key,
             RolePermissionPermissions.UserRole.View,
             "View User Roles",
             "Allows viewing user role assignments."
             ),
           
              new(
                  Group.Key,
             RolePermissionPermissions.UserRole.Replace,
             "Replace User Role",
             "Allows replacing a user's role."
             ),

            new(
                Group.Key,
                RolePermissionPermissions.UserRole.Delete,
                "Delete UserRole",
                "Allows deleting roles."
            ),

            new(Group.Key, 
                RolePermissionPermissions.UserRole.AssignUsers,
                "Assign UserRole",
                "Allows assigning roles to users."
            ),
            //================ UserPermissionGroup =================

          new(
              Group.Key,
          RolePermissionPermissions.UserPermissionGroup.View,
          "View User Permission Groups",
          "Allows viewing user permission group assignments."
            ),
            
            new(Group.Key,
                RolePermissionPermissions.UserPermissionGroup.Replace,
                "Replace User Permission Groups",
                "Allows replacing a user's permission groups."
            ),

            new(Group.Key,
                RolePermissionPermissions.UserPermissionGroup.Delete,
                "Delete UserPermissionsGroup",
                "Allows deleting roles."
            ),

            new(Group.Key,
                RolePermissionPermissions.UserPermissionGroup.AssignUsers,
                "Assign UserPermissionsGroup",
                "Allows assigning PermissionsGroup to users."
            ),
            //================ Roles =================

            new(Group.Key,
                RolePermissionPermissions.Role.View,
                "View Roles",
                "Allows viewing roles."
            ),

            new(Group.Key,
                RolePermissionPermissions.Role.Create,
                "Create Role",
                "Allows creating roles."
            ),

            new(Group.Key,
                RolePermissionPermissions.Role.Update,
                "Update Role",
                "Allows updating roles."
            ),

            new(Group.Key,
                RolePermissionPermissions.Role.Delete,
                "Delete Role",
                "Allows deleting roles."
            ),

            new(Group.Key,
                RolePermissionPermissions.Role.AssignUsers,
                "Assign Roles",
                "Allows assigning roles to users."
            ),

            //================ Permission Groups =================

            new(Group.Key,
                RolePermissionPermissions.PermissionGroup.View,
                "View Permission Groups",
                "Allows viewing permission groups."
            ),

            new(Group.Key,
                RolePermissionPermissions.PermissionGroup.Create,
                "Create Permission Group",
                "Allows creating permission groups."
            ),

            new(Group.Key,
                RolePermissionPermissions.PermissionGroup.Update,
                "Update Permission Group",
                "Allows updating permission groups."
            ),

            new(Group.Key,
                RolePermissionPermissions.PermissionGroup.Delete,
                "Delete Permission Group",
                "Allows deleting permission groups."
            ),

            //================ Permissions =================

            new(Group.Key,
                RolePermissionPermissions.Permission.View,
                "View Permissions",
                "Allows viewing permissions."
            ),
            new(Group.Key,
                RolePermissionPermissions.Permission.Create,
                "Create Permissions",
                "Allows creating permissions."
            ),
            new(Group.Key,
                RolePermissionPermissions.Permission.Update,
                "Update Permissions",
                "Allows updating permissions."
            ),
            new(Group.Key,
                RolePermissionPermissions.Permission.Delete,
                "deleting Permissions",
                "Allows deleting permissions."
            )
        ];
    }
}