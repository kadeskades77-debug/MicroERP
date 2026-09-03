using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

public class AttendanceLogDto
{
    public int Id { get; set; }

    public int AttendanceDeviceId { get; set; }

    public string DeviceEmployeeId { get; set; } = null!;

    public int? EmployeeId { get; set; }

    public DateTime LogTime { get; set; }

    public AttendanceLogType Type { get; set; }

    public bool IsProcessed { get; set; }
}