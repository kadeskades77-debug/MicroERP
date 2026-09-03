using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

public class CreateAttendanceCorrectionDto
{
    public int AttendanceRecordId { get; set; }


    public TimeOnly? NewCheckIn { get; set; }


    public TimeOnly? NewCheckOut { get; set; }

    public ShiftNumber ShiftNumber { get; set; }

    public AttendanceStatus? NewStatus { get; set; }


    public string Reason { get; set; } = null!;
}