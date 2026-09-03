using MicroERP.Domin.Common;

namespace MicroERP.Domin.Entities.Policies;

public class AttendancePolicy : BaseEntity
{
    public string Name { get; set; } = string.Empty;


    // =========================
    // خصومات الأيام
    // =========================

    // خصم كل يوم غياب
    public int AbsentPenaltyPoints { get; set; }


    // خصم كل يوم نصف دوام
    public int PartialAttendancePenaltyPoints { get; set; }


    // خصم كل يوم إجازة غير مدفوعة
    public decimal UnpaidLeavePenaltyPoints { get; set; }



    // =========================
    // الإجازات
    // =========================

    // 5 أيام إجازة سنوية سماح بالشهر
    public int MonthlyAnnualLeaveLimit { get; set; }


    // خصم لكل يوم بعد الحد
    public int AnnualLeavePenaltyPoints { get; set; }



    // 3 أيام إجازة مرضية سماح بالشهر
    public int MonthlySickLeaveLimit { get; set; }


    // خصم لكل يوم بعد الحد
    public int SickLeavePenaltyPoints { get; set; }



    // =========================
    // خصومات البصمة
    // =========================

    // نسيان الدخول
    public int MissingCheckInPenaltyPoints { get; set; }


    // نسيان الخروج
    public int MissingCheckOutPenaltyPoints { get; set; }



    // =========================
    // التأخير
    // =========================

    // أول نصف ساعة سماح
    public int LateGraceMinutes { get; set; }


    // كل نصف ساعة بعد السماح = درجة
    public int LateMinutesPerPenaltyPoint { get; set; }



    // =========================
    // الوقت الضائع
    // =========================

    // أول ساعة سماح
    public int LostTimeGraceMinutes { get; set; }


    // كل ساعة بعد السماح = درجة
    public int LostMinutesPerPenaltyPoint { get; set; }



    // =========================
    // الانصراف المبكر
    // =========================

    // عدد الأيام المسموح بها بالشهر
    public int MonthlyEarlyLeaveLimit { get; set; }


    // كل يوم بعد الحد = درجة
    public int EarlyLeavePenaltyPoints { get; set; }



    // =========================
    // الحضور
    // =========================

    // أقل دقائق لاعتبار اليوم مكتمل
    public int MinimumWorkMinutes { get; set; }



    // =========================
    // التقييم
    // =========================

    public int MinimumPerformanceScore { get; set; } = 0;


    public int MaximumPerformanceScore { get; set; } = 100;



    // =========================
    // السياسة الافتراضية
    // =========================

    public bool IsDefault { get; set; }
}