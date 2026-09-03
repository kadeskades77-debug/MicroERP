namespace MicroERP.Domin.Enums
{
    public enum AttendanceStatus
    {
        // حضر بشكل طبيعي
        Present = 1,

        // لم يحضر
        Absent = 2,

        //نص دوام 
        PartialAttendance = 3,

        // نسي تسجيل الدخول
        MissingCheckIn = 4,

        // نسي تسجيل الخروج
        MissingCheckOut = 5,

        // في إجازة معتمدة
        OnLeave = 6,

        // عطلة رسمية
        Holiday = 7,

        // عطلة أسبوعية
        Weekend = 8
    }
}
