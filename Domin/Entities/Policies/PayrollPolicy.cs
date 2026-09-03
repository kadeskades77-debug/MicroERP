using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.Policies
{
    public class PayrollPolicy : BaseEntity
    {
        // اسم السياسة
        public string Name { get; set; } = null!;

        // عدد أيام العمل بالشهر
        public int WorkingDays { get; set; }

        // عدد دقائق العمل في اليوم
        public int WorkingMinutesPerDay { get; set; }

        // الوقت الضائع المسموح شهريًا
        public int MonthlyFreeMinutes { get; set; }

        // -------------------------
        // سياسة الغياب
        // -------------------------

        // كم يوم راتب يخصم لكل يوم غياب
        // مثال:
        // 1 = يخصم يوم
        // 2 = يخصم يومين
        // 0.5 = نصف يوم
        public decimal AbsentDeductionFactor { get; set; } = 1m;

        // -------------------------
        // سياسة نصف الدوام
        // -------------------------

        // هل يطبق خصم نصف الدوام
        public bool EnablePartialAttendanceDeduction { get; set; }

        // طريقة الخصم
        public PartialAttendanceCalculationType PartialAttendanceCalculationType { get; set; }

        // إذا كانت بالنسب المئوية
        public decimal? PartialAttendancePercentage { get; set; }

        // إذا كانت بالأيام
        public decimal PartialAttendanceDeductionFactor { get; set; }

        public decimal PartialAttendanceWithPermissionPercentage { get; set; }

        public decimal PartialAttendanceWithoutPermissionPercentage { get; set; }

        // السياسة الافتراضية
        public bool IsDefault { get; set; }
    }
}
