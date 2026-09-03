using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

public class UpdateAttendanceDto
{
    public string? Notes { get; set; }


    public AttendanceStatus? Status { get; set; }
}