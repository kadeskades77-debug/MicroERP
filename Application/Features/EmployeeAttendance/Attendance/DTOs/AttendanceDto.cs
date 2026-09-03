using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

public class AttendanceDto
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public string DepartmentName { get; set; } = null!;

    public string? WorkScheduleName { get; set; }

    public DateOnly Date { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public int ExpectedMinutes { get; set; }

    public int WorkedMinutes { get; set; }

    public int LateMinutes { get; set; }

    public int EarlyLeaveMinutes { get; set; }

    public int LostTimeMinutes { get; set; }

    public bool IsMissingCheckIn { get; set; }

    public bool IsMissingCheckOut { get; set; }

    public AttendanceStatus Status { get; set; }

    public int TransactionsCount { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedOn { get; set; }
}