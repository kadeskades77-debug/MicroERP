using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

public class AttendanceDeviceDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string DeviceCode { get; set; } = null!;

    public string? IpAddress { get; set; }

    public string? Location { get; set; }

    public bool IsActiveDevice { get; set; }

    public DeviceConnectionType ConnectionType { get; set; }
}