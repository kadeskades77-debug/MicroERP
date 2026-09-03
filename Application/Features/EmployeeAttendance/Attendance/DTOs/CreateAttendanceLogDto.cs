using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

public class CreateAttendanceLogDto
{
    public int AttendanceDeviceId { get; set; }


    // رقم الموظف كما هو مسجل داخل جهاز البصمة
    public string DeviceEmployeeId { get; set; } = null!;


    // وقت البصمة القادم من الجهاز
    public DateTime LogTime { get; set; }


    public AttendanceLogType Type { get; set; }


    // رقم العملية القادم من الجهاز لمنع التكرار
    public string? TransactionId { get; set; }


    public string? RawData { get; set; }
}