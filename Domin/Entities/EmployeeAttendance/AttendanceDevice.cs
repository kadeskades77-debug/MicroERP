using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.EmployeeAttendance;

public class AttendanceDevice : BaseEntity
{
    public string Name { get; set; } = null!;

    // رقم الجهاز أو الرقم التعريفي
    public string DeviceCode { get; set; } = null!;


    // IP الخاص بالجهاز إذا كان داخل الشبكة
    public string? IpAddress { get; set; }


    // موقع الجهاز
    public string? Location { get; set; }


    // هل الجهاز فعال
    public bool IsActiveDevice { get; set; } = true;


    // نوع الاتصال
    public DeviceConnectionType ConnectionType { get; set; }


    public ICollection<AttendanceLog> AttendanceLogs { get; set; }
        = new List<AttendanceLog>();
    // لكي لا يتكرر الجهاز
    public ICollection<EmployeeAttendanceDevice> EmployeeMappings { get; set; }
    = new List<EmployeeAttendanceDevice>();
}