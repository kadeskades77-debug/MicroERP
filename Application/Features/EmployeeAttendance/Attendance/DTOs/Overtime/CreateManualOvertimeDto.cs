using MicroERP.Domin.Enums;
using System.ComponentModel.DataAnnotations;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;

public class CreateManualOvertimeDto
{
    public int EmployeeId { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public OvertimeType Type { get; set; }

    public string? Reason { get; set; }
}