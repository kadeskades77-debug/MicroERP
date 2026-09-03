namespace MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.DTOs;

public class WorkScheduleDto
{
    public int Id { get; set; }


    public string Name { get; set; } = null!;


    // الفترة الأولى
    public TimeOnly FirstShiftStart { get; set; }

    public TimeOnly FirstShiftEnd { get; set; }


    // الفترة الثانية
    public TimeOnly? SecondShiftStart { get; set; }

    public TimeOnly? SecondShiftEnd { get; set; }



    public int LateGraceMinutes { get; set; }


    public int EarlyLeaveGraceMinutes { get; set; }


    public int MinimumWorkMinutes { get; set; }


    public bool IsDefault { get; set; }
}