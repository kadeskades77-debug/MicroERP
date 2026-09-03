using MicroERP.Application.Common.Interfaces;
using MicroERP.Domin.Entities;
using MicroERP.Domin.Entities.Policies;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Seed;

public static class AttendancePolicySeeder
{
    public static async Task SeedAsync(
        IApplicationDbContext context,
        CancellationToken cancellationToken = default)
    {
        var exists =
            await context.AttendancePolicies
            .AnyAsync(
                x => x.IsDefault,
                cancellationToken);


        if (exists)
            return;



        var policy = new AttendancePolicy
        {
            Name = "Default Attendance Policy",



            // =========================
            // خصومات الأيام
            // =========================


            // غياب يوم كامل = 3 درجات
            AbsentPenaltyPoints = 3,


            // نصف دوام = درجة واحدة
            PartialAttendancePenaltyPoints = 1,



            // غير مدفوع = 1.5 درجة
            UnpaidLeavePenaltyPoints = 1.5m,



            // =========================
            // الإجازات
            // =========================


            // 5 أيام سنوية سماح بالشهر
            MonthlyAnnualLeaveLimit = 5,


            // كل يوم زائد = درجة
            AnnualLeavePenaltyPoints = 1,



            // 3 أيام مرضية سماح بالشهر
            MonthlySickLeaveLimit = 3,


            // كل يوم زائد = درجة
            SickLeavePenaltyPoints = 1,



            // =========================
            // التأخير
            // =========================


            // أول نصف ساعة سماح
            LateGraceMinutes = 30,


            // كل نصف ساعة بعدها = درجة
            LateMinutesPerPenaltyPoint = 30,



            // =========================
            // الوقت الضائع
            // =========================


            // أول ساعة سماح
            LostTimeGraceMinutes = 60,


            // كل ساعة بعدها = درجة
            LostMinutesPerPenaltyPoint = 60,



            // =========================
            // الخروج المبكر
            // =========================


            // 5 أيام خروج مبكر سماح
            MonthlyEarlyLeaveLimit = 5,


            // كل يوم بعد الحد = درجة
            EarlyLeavePenaltyPoints = 1,



            // =========================
            // البصمة
            // =========================


            MissingCheckInPenaltyPoints = 2,

            MissingCheckOutPenaltyPoints = 2,



            // =========================
            // الحضور
            // =========================


            MinimumWorkMinutes = 420,



            // =========================
            // التقييم
            // =========================


            MinimumPerformanceScore = 0,

            MaximumPerformanceScore = 100,



            IsDefault = true
        };


        await context.AttendancePolicies
            .AddAsync(
                policy,
                cancellationToken);


        await context.SaveChangesAsync(
            cancellationToken);
    }
}