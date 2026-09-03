using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

public class AttendanceCorrectionDto
{
    public int Id { get; set; }


    public int AttendanceRecordId { get; set; }


    public DateOnly Date { get; set; }


    public int EmployeeId { get; set; }


    public string EmployeeName { get; set; } = null!;



    public TimeOnly? OldCheckIn { get; set; }

    public TimeOnly? NewCheckIn { get; set; }



    public TimeOnly? OldCheckOut { get; set; }

    public TimeOnly? NewCheckOut { get; set; }



    public AttendanceStatus? OldStatus { get; set; }

    public AttendanceStatus? NewStatus { get; set; }



    public string Reason { get; set; } = null!;



    public string RequestedByUserId { get; set; } = null!;



    public AttendanceCorrectionStatus Status { get; set; }



    public DateTime CreatedOn { get; set; }
}