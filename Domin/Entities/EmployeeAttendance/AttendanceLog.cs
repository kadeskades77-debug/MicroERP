using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.EmployeeAttendance;

public class AttendanceLog : BaseEntity
{
    public int AttendanceDeviceId { get; set; }

    public AttendanceDevice AttendanceDevice { get; set; } = null!;


    // رقم الموظف في جهاز البصمة
    public string DeviceEmployeeId { get; set; } = null!;


    // الموظف بعد عملية المطابقة
    public int? EmployeeId { get; set; }

    public Employee? Employee { get; set; }


    // وقت البصمة من الجهاز
    public DateTime LogTime { get; set; }


    // نوع الحركة
    public AttendanceLogType Type { get; set; }


    // هل تمت معالجتها وتحويلها إلى AttendanceRecord
    public bool IsProcessed { get; set; } = false;


    // معرف العملية من الجهاز لمنع التكرار
    public string? TransactionId { get; set; }


    public string? RawData { get; set; }
}