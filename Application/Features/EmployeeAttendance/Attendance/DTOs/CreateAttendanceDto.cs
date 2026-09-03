namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

public class CreateAttendanceDto
{
    public int EmployeeId { get; set; }


    public DateOnly Date { get; set; }


    public string? Notes { get; set; }
}