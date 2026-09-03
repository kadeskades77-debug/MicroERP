namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;

public class CreateAttendancePolicyDto
{
    public string Name { get; set; } = string.Empty;


    public int AbsentPenaltyPoints { get; set; }


    public int PartialAttendancePenaltyPoints { get; set; }


    public int MissingCheckInPenaltyPoints { get; set; }


    public int MissingCheckOutPenaltyPoints { get; set; }


    public int LateMinutesPerPenaltyPoint { get; set; }

    public int LostMinutesPerPenaltyPoint { get; set; }


    public int MinimumWorkMinutes { get; set; }


    public int MinimumPerformanceScore { get; set; }


    public int MaximumPerformanceScore { get; set; }


    public bool IsDefault { get; set; }
}