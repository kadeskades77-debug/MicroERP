using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.EmployeeAttendance;

public class AttendanceTransaction : BaseEntity
{
    public int AttendanceRecordId { get; set; }

    public AttendanceRecord AttendanceRecord { get; set; } = null!;



    // الجهاز (اختياري)
    public int? AttendanceDeviceId { get; set; }

    public AttendanceDevice? AttendanceDevice { get; set; }



    // سجل الجهاز (اختياري)
    public int? AttendanceLogId { get; set; }

    public AttendanceLog? AttendanceLog { get; set; }



    // وقت البصمة
    public DateTime TransactionTime { get; set; }



    // دخول أو خروج
    public AttendanceTransactionType Type { get; set; }



    // الفترة الأولى أو الثانية
    public ShiftNumber ShiftNumber { get; set; }
    public string? Notes { get; set; }
}