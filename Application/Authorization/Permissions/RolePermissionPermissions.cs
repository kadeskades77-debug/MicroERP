namespace MicroERP.Application.Authorization.Permissions
{

    public static class RolePermissionPermissions
    {
        public static class Role
        {
            public const string View = "RolePermission.Role.View";
            public const string Create = "RolePermission.Role.Create";
            public const string Update = "RolePermission.Role.Update";
            public const string Delete = "RolePermission.Role.Delete";
            public const string AssignUsers = "RolePermission.Role.AssignUsers";
        }
        public static class UserRole
        {
            public const string View = "RolePermission.UserRole.View";
            public const string Replace = "RolePermission.UserRole.ReplaceRole";
            public const string Delete = "RolePermission.UserRole.Delete";
            public const string AssignUsers = "RolePermission.UserRole.AssignUsers";
        }

        public static class PermissionGroup
        {
            public const string View = "RolePermission.PermissionGroup.View";
            public const string Create = "RolePermission.PermissionGroup.Create";
            public const string Update = "RolePermission.PermissionGroup.Update";
            public const string Delete = "RolePermission.PermissionGroup.Delete";
        }
        public static class UserPermissionGroup
        {
            public const string View = "RolePermission.UserPermissionGroup.View";
            public const string Replace = "RolePermission.UserPermissionGroup.ReplacePermissionsGroup";
            public const string Delete = "RolePermission.UserPermissionGroup.Delete";
            public const string AssignUsers = "RolePermission.UserPermissionGroup.AssignUsers";
        }

        public static class Permission
        {
            public const string View = "RolePermission.Permission.View";
            public const string Create = "RolePermission.Permission.Create";
            public const string Update = "RolePermission.Permission.Update";
            public const string Delete = "RolePermission.Permission.Delete";
        }
    }
}
