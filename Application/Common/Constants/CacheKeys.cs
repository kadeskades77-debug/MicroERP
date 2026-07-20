namespace MicroERP.Application.Common.Constants;

public static class CacheKeys
{
    public const string UserPermissions = "UserPermissions";

    public const string RolePermissions = "RolePermissions";

    public static string UserPermissionsKey(string userId)
        => $"{UserPermissions}:{userId}";

    public static string RolePermissionsKey(string roleId)
        => $"{RolePermissions}:{roleId}";
}