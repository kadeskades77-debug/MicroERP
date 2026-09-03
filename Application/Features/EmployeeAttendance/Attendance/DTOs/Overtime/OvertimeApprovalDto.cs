namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;

public class OvertimeApprovalDto
{
    public bool Approve { get; set; }

    public string? RejectionReason { get; set; }
}