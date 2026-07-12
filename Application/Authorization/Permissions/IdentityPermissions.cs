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

        public const string Lock =
            "Identity.User.Lock";

        public const string Unlock =
            "Identity.User.Unlock";

        public const string ResetPassword =
            "Identity.User.ResetPassword";

        public const string Delete =
            "Identity.User.Delete";

        public const string Activate =
            "Identity.User.Activate";

        public const string Deactivate =
            "Identity.User.Deactivate";
    }
}