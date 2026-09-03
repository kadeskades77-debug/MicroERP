namespace MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.DTOs;

public class UpdateWorkScheduleDto
{
    public string? Name { get; set; }

    public TimeOnly? FirstShiftStart { get; set; }

    public TimeOnly? FirstShiftEnd { get; set; }

    public TimeOnly? SecondShiftStart { get; set; }

    public TimeOnly? SecondShiftEnd { get; set; }

    public int? LateGraceMinutes { get; set; }

    public int? EarlyLeaveGraceMinutes { get; set; }

    public int? MinimumWorkMinutes { get; set; }

    public bool? IsDefault { get; set; }
}