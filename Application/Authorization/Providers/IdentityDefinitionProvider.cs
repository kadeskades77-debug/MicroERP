using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Authorization.Permissions;

namespace MicroERP.Application.Authorization.Providers;

public class IdentityDefinitionProvider
    : IMultiPermissionDefinitionProvider
{
    public IEnumerable<PermissionGroupDefinition> Groups =>
    [
        new(
            key: "User",
            name: "Users",
            description: "System user management permissions.",
            isSystem: true),

        new(
            key: "UserPassword",
            name: "User Password",
            description: "User password management permissions.",
            isSystem: true),

        new(
            key: "UserLock",
            name: "User Lock",
            description: "User account lock management permissions.",
            isSystem: true),

        new(
            key: "UserActivation",
            name: "User Activation",
            description: "User account activation management permissions.",
            isSystem: true)
    ];

    public IEnumerable<PermissionDefinition> GetPermissions()
    {
        // User
        yield return new(
            "User",
            IdentityPermissions.User.View,
            "View Users",
            "Allows viewing system users."
        );

        yield return new(
            "User",
            IdentityPermissions.User.Register,
            "Register Users",
            "Allows registering new system users."
        );

        yield return new(
            "User",
            IdentityPermissions.User.ChangeEmail,
            "Change User Email",
            "Allows changing the email address of system users."
        );

        yield return new(
            "User",
            IdentityPermissions.User.Delete,
            "Delete Users",
            "Allows deleting system users."
        );

        // User Password
        yield return new(
            "UserPassword",
            IdentityPermissions.UserPassword.Reset,
            "Reset User Password",
            "Allows resetting passwords for system users."
        );

        // User Lock
        yield return new(
            "UserLock",
            IdentityPermissions.UserLock.Lock,
            "Lock Users",
            "Allows locking system user accounts."
        );

        yield return new(
            "UserLock",
            IdentityPermissions.UserLock.Unlock,
            "Unlock Users",
            "Allows unlocking system user accounts."
        );

        // User Activation
        yield return new(
            "UserActivation",
            IdentityPermissions.UserActivation.Activate,
            "Activate Users",
            "Allows activating system user accounts."
        );

        yield return new(
            "UserActivation",
            IdentityPermissions.UserActivation.Deactivate,
            "Deactivate Users",
            "Allows deactivating system user accounts."
        );
    }
}