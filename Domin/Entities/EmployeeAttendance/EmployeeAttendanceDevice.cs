using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;

namespace MicroERP.Domin.Entities.EmployeeAttendance;

public class EmployeeAttendanceDevice : BaseEntity
{
    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;


    public int AttendanceDeviceId { get; set; }

    public AttendanceDevice AttendanceDevice { get; set; } = null!;


    // رقم الموظف داخل جهاز البصمة
    public string DeviceEmployeeId { get; set; } = null!;
}