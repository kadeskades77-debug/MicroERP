using MicroERP.Domin.Common;

namespace MicroERP.Domin.Entities.EmployeeAttendance;

public class WorkSchedule : BaseEntity
{
    public string Name { get; set; } = null!;


    // الفترة الأولى
    public TimeOnly FirstShiftStart { get; set; }

    public TimeOnly FirstShiftEnd { get; set; }


    // الفترة الثانية اختيارية
    public TimeOnly? SecondShiftStart { get; set; }

    public TimeOnly? SecondShiftEnd { get; set; }


    // سماح التأخير
    public int LateGraceMinutes { get; set; } = 0;


    // سماح الخروج المبكر
    public int EarlyLeaveGraceMinutes { get; set; } = 0;


    // أقل وقت عمل مطلوب
    public int MinimumWorkMinutes { get; set; }

    public bool IsFirstShiftRequired { get; set; } = true;

    public bool IsSecondShiftRequired { get; set; } = true;

    // جدول الدوام الافتراضي
    public bool IsDefault { get; set; }
}