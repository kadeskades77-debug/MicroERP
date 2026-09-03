using MicroERP.Domin.Enums;

namespace MicroERP.Application.Common.Excel.Helpers
{
    public static class EnumDisplayHelper
    {
        public static string GetOvertimeTypeName(
            OvertimeType type)
        {
            return type switch
            {
                OvertimeType.Normal => "عادي",
                OvertimeType.Weekend => "عطلة",
                OvertimeType.Holiday => "إجازة",
                _ => type.ToString()
            };
        }

        public static string GetOvertimeStatusName(
            OvertimeStatus status)
        {
            return status switch
            {
                OvertimeStatus.Pending => "قيد الانتظار",
                OvertimeStatus.Approved => "معتمد",
                OvertimeStatus.Rejected => "مرفوض",
                _ => status.ToString()
            };
        }

        public static string GetOvertimeSourceName(
            OvertimeSource source)
        {
            return source switch
            {
                OvertimeSource.Attendance => "الحضور",
                OvertimeSource.Manual => "يدوي",
                _ => source.ToString()
            };
        }

        public static string GetBooleanName(bool value)
        {
            return value
                ? "نعم"
                : "لا";
        }
    }
}
