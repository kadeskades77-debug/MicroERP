using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

public class UpdateAttendanceDeviceDto
{
    public string? Name { get; set; }

    public string? IpAddress { get; set; }

    public string? Location { get; set; }

    public bool? IsActiveDevice { get; set; }

    public DeviceConnectionType? ConnectionType { get; set; }
}