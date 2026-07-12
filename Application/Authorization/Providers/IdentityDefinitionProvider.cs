using MicroERP.Application.Authorization.Interfaces;
using MicroERP.Application.Authorization.Permissions;

namespace MicroERP.Application.Authorization.Providers;

public class IdentityDefinitionProvider
    : IPermissionDefinitionProvider
{
    public PermissionGroupDefinition Group =>
        new(
            key: "Identity",
            name: "Identity Management",
            description: "User account management permissions.",
            isSystem: true
        );


    public IEnumerable<PermissionDefinition> GetPermissions()
    {
        return
        [
            new(
                Group.Key,
                IdentityPermissions.User.View,
                "View Users",
                "Allows viewing system users."
            ),

            new(
                Group.Key,
                IdentityPermissions.User.Register,
                "Register User",
                "Allows creating new users."
            ),

            new(
                Group.Key,
                IdentityPermissions.User.ChangeEmail,
                "Change User Email",
                "Allows changing user email."
            ),

            new(
                Group.Key,
                IdentityPermissions.User.Lock,
                "Lock User",
                "Allows locking user accounts."
            ),

            new(
                Group.Key,
                IdentityPermissions.User.Unlock,
                "Unlock User",
                "Allows unlocking user accounts."
            ),

            new(
                Group.Key,
                IdentityPermissions.User.ResetPassword,
                "Reset User Password",
                "Allows resetting user password."
            ),

            new(
                Group.Key,
                IdentityPermissions.User.Delete,
                "Delete User",
                "Allows deleting users."
            ),

            new(
                Group.Key,
                IdentityPermissions.User.Activate,
                "Activate User",
                "Allows activating users."
            ),

            new(
                Group.Key,
                IdentityPermissions.User.Deactivate,
                "Deactivate User",
                "Allows deactivating users."
            )
        ];
    }
}