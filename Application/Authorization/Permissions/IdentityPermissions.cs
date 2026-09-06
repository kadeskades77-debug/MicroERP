namespace MicroERP.Application.Authorization.Permissions;

public static class IdentityPermissions
{
    public static class User
    {
        public const string View =
            "Identity.User.View";

        public const string Register =
            "Identity.User.Register";

        public const string ChangeEmail =
            "Identity.User.ChangeEmail";

        public const string Delete =
            "Identity.User.Delete";
    }

    public static class UserPassword
    {
        public const string Reset =
            "Identity.UserPassword.Reset";
    }

    public static class UserLock
    {
        public const string Lock =
            "Identity.UserLock.Lock";

        public const string Unlock =
            "Identity.UserLock.Unlock";
    }

    public static class UserActivation
    {
        public const string Activate =
            "Identity.UserActivation.Activate";

        public const string Deactivate =
            "Identity.UserActivation.Deactivate";
    }
}