namespace MicroERP.Domain.Audit;

public static class AuditActions
{
    public const string Create = nameof(Create);

    public const string Update = nameof(Update);

    public const string Delete = nameof(Delete);

    public const string Restore = nameof(Restore);

    public const string Activate = nameof(Activate);

    public const string Deactivate = nameof(Deactivate);

    public const string Login = nameof(Login);

    public const string Logout = nameof(Logout);

    public const string Register = nameof(Register);

    public const string ChangePassword = nameof(ChangePassword);

    public const string ResetPassword = nameof(ResetPassword);

    public const string ChangeEmail = nameof(ChangeEmail);

    public const string Lock = nameof(Lock);

    public const string Unlock = nameof(Unlock);

    public const string AssignRole = nameof(AssignRole);

    public const string ReplaceRole = nameof(ReplaceRole);

    public const string DeleteUserRole = nameof(DeleteUserRole);

    public const string AssignPermissionGroup = nameof(AssignPermissionGroup);

    public const string ReplacePermissionGroup = nameof(ReplacePermissionGroup);

    public const string RemovePermissionGroup = nameof(RemovePermissionGroup);

    public const string AssignManager = nameof(AssignManager);
}