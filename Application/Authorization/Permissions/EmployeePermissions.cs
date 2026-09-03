namespace MicroERP.Application.Authorization.Permissions;

public static class EmployeePermissions
{
    public static class Profile
    {
        public const string View =
            "Profile.ViewProfile";

        public const string MyAttendance = "Attendance.My";

        public const string ChangePassword =
            "Profile.ChangePassword";

    }
}