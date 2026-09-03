using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.EmployeeAttendance;

public class AttendanceCorrection : BaseEntity
{
    public int AttendanceRecordId { get; set; }

    public AttendanceRecord AttendanceRecord { get; set; } = null!;


    // القيم القديمة
    public TimeOnly? OldCheckIn { get; set; }

    public TimeOnly? OldCheckOut { get; set; }

    public AttendanceStatus? OldStatus { get; set; }



    // القيم الجديدة
    public TimeOnly? NewCheckIn { get; set; }

    public TimeOnly? NewCheckOut { get; set; }

    public AttendanceStatus? NewStatus { get; set; }

    public ShiftNumber ShiftNumber { get; set; }

    public string Reason { get; set; } = null!;


    // من طلب التصحيح
    public string RequestedByUserId { get; set; } = null!;


    // من اعتمد أو رفض
    public string? ApprovedByUserId { get; set; }


    public DateTime? ApprovedOn { get; set; }


    public string? RejectionReason { get; set; }


    public AttendanceCorrectionStatus Status { get; set; }
}