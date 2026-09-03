namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

public class RejectAttendanceCorrectionDto
{
    public int CorrectionId { get; set; }


    public string RejectionReason { get; set; } = null!;
}