using MicroERP.Application.Features.Payrolls.SalaryComponents.Services;
using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Seed;

public static class SalaryComponentSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        if (await context.SalaryComponents.AnyAsync())
            return;


        var components = new List<SalaryComponent>
        {

            // ==========================
            // Basic Salary
            // ==========================

            new()
            {
                NameAr = "الراتب الأساسي",
                NameEn = "Basic Salary",
                Code = SalaryComponentCodes.Basic,

                Type = SalaryComponentType.Allowance,

                CalculationType =
                    CalculationType.FixedAmount,

                IsTaxable = true,

                IsAttendanceRelated = false,

                IsDefault = true
            },


            // ==========================
           // Payroll Adjustment
           // ==========================
           
            new()
            {
               NameAr = "تعديل راتب",
           
               NameEn = "Payroll Adjustment",
           
               Code = "ADJUSTMENT",
           
               Type = SalaryComponentType.Deduction,
           
               CalculationType =
                   CalculationType.FixedAmount,
           
               IsTaxable = false,
           
               IsAttendanceRelated = false,
           
               IsDefault = false
            },

            // ==========================
            // Allowances
            // ==========================

            new()
            {
                NameAr = "بدل السكن",
                NameEn = "Housing Allowance",

                Code = "HOUSING",

                Type = SalaryComponentType.Allowance,

                CalculationType =
                    CalculationType.Percentage,

                IsTaxable = true,

                IsAttendanceRelated = false,

                IsDefault = true
            },


            new()
            {
                NameAr = "بدل النقل",
                NameEn = "Transport Allowance",

                Code = "TRANSPORT",

                Type = SalaryComponentType.Allowance,

                CalculationType =
                    CalculationType.FixedAmount,

                IsTaxable = false,

                IsAttendanceRelated = false,

                IsDefault = true
            },


            new()
            {
                NameAr = "بدل الطعام",
                NameEn = "Meal Allowance",

                Code = "MEAL",

                Type = SalaryComponentType.Allowance,

                CalculationType =
                    CalculationType.FixedAmount,

                IsTaxable = false,

                IsAttendanceRelated = false,

                IsDefault = false
            },


            new()
            {
                NameAr = "بدل اتصال",
                NameEn = "Communication Allowance",

                Code = "COMMUNICATION",

                Type = SalaryComponentType.Allowance,

                CalculationType =
                    CalculationType.FixedAmount,

                IsTaxable = false,

                IsAttendanceRelated = false,

                IsDefault = false
            },


            // ==========================
            // Deductions
            // ==========================


            new()
            {
                NameAr = "التأمين",
                NameEn = "Insurance",

                Code = "INSURANCE",

                Type = SalaryComponentType.Deduction,

                CalculationType =
                    CalculationType.Percentage,

                IsTaxable = false,

                IsAttendanceRelated = false,

                IsDefault = true
            },


            new()
            {
                NameAr = "الضريبة",
                NameEn = "Tax",

                Code = "TAX",

                Type = SalaryComponentType.Deduction,

                CalculationType =
                    CalculationType.Percentage,

                IsTaxable = false,

                IsAttendanceRelated = false,

                IsDefault = false
            },


            // ==========================
            // Attendance Deductions
            // ==========================

            new()
             {
                 NameAr = "خصم الغياب",
                 NameEn = "Absence Deduction",
             
                 Code = "ABSENT",
             
                 Type = SalaryComponentType.Deduction,
             
                 CalculationType =
                     CalculationType.FixedAmount,
             
                 IsTaxable = false,
             
                 IsAttendanceRelated = true,
             
                 IsDefault = false
             },
             
             new()
             {
                 NameAr = "خصم نصف الدوام",
                 NameEn = "Partial Attendance Deduction",
             
                 Code = "PARTIAL_ATTENDANCE",
             
                 Type = SalaryComponentType.Deduction,
             
                 CalculationType =
                     CalculationType.FixedAmount,
             
                 IsTaxable = false,
             
                 IsAttendanceRelated = true,
             
                 IsDefault = false
             },

            new()
            {
                NameAr = "خصم التأخير",
                NameEn = "Late Deduction",

                Code = "LATE",

                Type = SalaryComponentType.Deduction,

                CalculationType =
                    CalculationType.FixedAmount,

                IsTaxable = false,

                IsAttendanceRelated = true,

                IsDefault = false
            },


            new()
            {
                NameAr = "خصم الخروج المبكر",
                NameEn = "Early Leave Deduction",

                Code = "EARLY_LEAVE",

                Type = SalaryComponentType.Deduction,

                CalculationType =
                    CalculationType.FixedAmount,

                IsTaxable = false,

                IsAttendanceRelated = true,

                IsDefault = false
            },


            new()
            {
                NameAr = "خصم الوقت الضائع",
                NameEn = "Lost Time Deduction",

                Code = "LOST_TIME",

                Type = SalaryComponentType.Deduction,

                CalculationType =
                    CalculationType.FixedAmount,

                IsTaxable = false,

                IsAttendanceRelated = true,

                IsDefault = false
            },


            // ==========================
            // Overtime
            // ==========================


            new()
            {
                NameAr = "العمل الإضافي",
                NameEn = "Overtime",

                Code = "OVERTIME",

                Type = SalaryComponentType.Allowance,

                CalculationType =
                    CalculationType.FixedAmount,

                IsTaxable = true,

                IsAttendanceRelated = true,

                IsDefault = false
            },


            // ==========================
            // Bonuses
            // ==========================


            new()
            {
                NameAr = "مكافأة",
                NameEn = "Bonus",

                Code = "BONUS",

                Type = SalaryComponentType.Allowance,

                CalculationType =
                    CalculationType.FixedAmount,

                IsTaxable = true,

                IsAttendanceRelated = false,

                IsDefault = false
            }
        };


        await context.SalaryComponents
            .AddRangeAsync(components);


        await context.SaveChangesAsync();
    }
}