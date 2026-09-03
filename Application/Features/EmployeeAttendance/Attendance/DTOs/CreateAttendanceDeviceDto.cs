using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

public class CreateAttendanceDeviceDto
{
    public string Name { get; set; } = null!;

    public string DeviceCode { get; set; } = null!;

    public string? IpAddress { get; set; }

    public string? Location { get; set; }

    public DeviceConnectionType ConnectionType { get; set; }
}