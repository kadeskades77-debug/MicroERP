
using MicroERP.Application.Common.Interfaces;
using MicroERP.Domin.Entities;
using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Entities.Policies;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace MicroERP.Persistence.Seed
{
    public static class PayrollPolicySeeder
    {
        public static async Task Seed(IApplicationDbContext context)
        {

            var exists =
            await context.PayrollPolicys
            .AnyAsync(
                x => x.IsDefault);


            if (exists)
                return;

            var policy=
                new PayrollPolicy
                {
                    Name = "Default Payroll Policy",

                    // عدد أيام العمل بالشهر
                    WorkingDays = 26,

                    // عدد دقائق العمل اليومية (8 ساعات)
                    WorkingMinutesPerDay = 8 * 60,

                    // الوقت الضائع المسموح به شهريًا (60 دقيقة)
                    MonthlyFreeMinutes = 60,

                    // ==========================
                    // سياسة الغياب
                    // ==========================

                    // كل يوم غياب يخصم 3 أيام
                    AbsentDeductionFactor = 3m,

                    // ==========================
                    // سياسة نصف الدوام
                    // ==========================

                    EnablePartialAttendanceDeduction = true,

                    PartialAttendanceCalculationType =
        PartialAttendanceCalculationType.ByPercentage,

                    // بإذن
                    PartialAttendanceWithPermissionPercentage = 50m,

                    // بدون إذن
                    PartialAttendanceWithoutPermissionPercentage = 75m,

                    // السياسة الافتراضية
                    IsDefault = true,

                    IsActive = true
                };

            await context.PayrollPolicys
    .AddAsync(
        policy);



            await context.SaveChangesAsync();

        }
     }
}
