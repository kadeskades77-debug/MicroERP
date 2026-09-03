using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Entities.Policies;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;

public class AttendancePerformanceService
    : IAttendancePerformanceService
{
    private readonly IApplicationDbContext _context;
    private readonly AttendancePerformanceCalculator _calculator;


    public AttendancePerformanceService(
        IApplicationDbContext context,
        AttendancePerformanceCalculator calculator)
    {
        _context = context;
        _calculator = calculator;
    }



 
    public async Task<Result<AttendancePerformanceDto>> CalculateAsync(
    int employeeId,
    int year,
    int month,
    CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Employee
        // =========================================================

        var employee =
            await _context.Employees
                .AsNoTracking()
                .Where(x => x.Id == employeeId)
                .Select(x => new
                {
                    x.Id,
                    Name = x.User.FullName
                })
                .FirstOrDefaultAsync(
                    cancellationToken);


        if (employee == null)
        {
            return Result<AttendancePerformanceDto>
                .Failure(
                    "Employee not found.");
        }


        // =========================================================
        // Attendance Policy
        // =========================================================

        var policy =
            await _context.AttendancePolicies
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.IsDefault,
                    cancellationToken);


        if (policy == null)
        {
            return Result<AttendancePerformanceDto>
                .Failure(
                    "Attendance policy not found.");
        }


        // =========================================================
        // Period
        // =========================================================

        var (startDate, endDate) =
            GetMonthRange(
                year,
                month);


        // =========================================================
        // Attendance Records
        // =========================================================

        var records =
            await _context.AttendanceRecords
                .AsNoTracking()
                .Where(x =>
                    x.EmployeeId == employeeId &&

                    x.Date >= startDate &&

                    x.Date <= endDate)
                .ToListAsync(
                    cancellationToken);


        if (records.Count == 0)
        {
            return Result<AttendancePerformanceDto>
                .Failure(
                    "No attendance records found.");
        }


        // =========================================================
        // Approved Leaves
        // =========================================================
        //
        // نرسل الإجازات إلى Calculator.
        //
        // Calculator هو المسؤول عن:
        //
        // Annual
        // Sick
        // Emergency
        // Unpaid
        //
        // وحساب الخصومات والتقييم النهائي.
        // =========================================================

        var leaves =
            await _context.EmployeeLeaves
                .AsNoTracking()
                .Where(x =>
                    x.EmployeeId == employeeId &&

                    x.Status == LeaveStatus.Approved &&

                    x.StartDate <= endDate &&

                    x.EndDate >= startDate)
                .ToListAsync(
                    cancellationToken);


        // =========================================================
        // Calculate Performance
        // =========================================================
        var result =
            await _calculator.CalculateAsync(
                records,
                leaves,
                startDate,
                endDate,
                policy);


        // =========================================================
        // Get Existing Performance
        // =========================================================

        var performance =
            await _context.AttendancePerformances
                .FirstOrDefaultAsync(
                    x =>
                        x.EmployeeId == employeeId &&

                        x.Year == year &&

                        x.Month == month,
                    cancellationToken);


        // =========================================================
        // Create Performance
        // =========================================================

        if (performance == null)
        {
            performance =
                new AttendancePerformance
                {
                    EmployeeId =
                        employeeId,

                    Year =
                        year,

                    Month =
                        month
                };


            await _context.AttendancePerformances
                .AddAsync(
                    performance,
                    cancellationToken);
        }


        // =========================================================
        // Attendance Score
        // =========================================================

        performance.AttendanceScore =
            result.FinalScore;


        // =========================================================
        // Attendance Statistics
        // =========================================================

        performance.PresentDays =
            result.PresentDays;


        performance.AbsentDays =
            result.AbsentDays;


        performance.PartialAttendanceDays =
            result.PartialAttendanceDays;


        performance.MissingCheckInCount =
            result.MissingCheckInCount;


        performance.MissingCheckOutCount =
            result.MissingCheckOutCount;


        // =========================================================
        // Leave Statistics
        // =========================================================

        performance.AnnualLeaveDays =
            result.AnnualLeaveDays;


        performance.SickLeaveDays =
            result.SickLeaveDays;


        performance.EmergencyLeaveDays =
            result.EmergencyLeaveDays;


        performance.UnpaidLeaveDays =
            result.UnpaidLeaveDays;


        // =========================================================
        // Time Statistics
        // =========================================================

        performance.TotalLateMinutes =
            result.TotalLateMinutes;


        performance.TotalEarlyLeaveMinutes =
            result.TotalEarlyLeaveMinutes;


        performance.EarlyLeaveDays =
            result.EarlyLeaveDays;


        performance.TotalLostTimeMinutes =
            result.TotalLostTimeMinutes;


        // =========================================================
        // Penalties
        // =========================================================

        performance.TotalPenaltyPoints =
            result.TotalPenaltyPoints;


        // =========================================================
        // Save
        // =========================================================

        await _context.SaveChangesAsync(
            cancellationToken);


        // =========================================================
        // Return
        // =========================================================

        return Result<AttendancePerformanceDto>
            .Succeeded(
                Map(
                    performance,
                    employee.Name));
    }

    public async Task<Result<List<AttendancePerformanceDto>>>
    CalculateAllAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Validate Year / Month
        // =========================================================

        if (year <= 0)
        {
            return Result<List<AttendancePerformanceDto>>
                .Failure(
                    "Year must be greater than zero.");
        }

        if (month < 1 || month > 12)
        {
            return Result<List<AttendancePerformanceDto>>
                .Failure(
                    "Month must be between 1 and 12.");
        }


        // =========================================================
        // Attendance Policy
        // =========================================================

        var policy =
            await _context.AttendancePolicies
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.IsDefault,
                    cancellationToken);


        if (policy == null)
        {
            return Result<List<AttendancePerformanceDto>>
                .Failure(
                    "Attendance policy not found.");
        }


        // =========================================================
        // Period
        // =========================================================

        var (startDate, endDate) =
            GetMonthRange(
                year,
                month);


        // =========================================================
        // Employees
        // =========================================================

        var employees =
            await _context.Employees
                .AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    Name = x.User.FullName
                })
                .ToListAsync(
                    cancellationToken);


        if (employees.Count == 0)
        {
            return Result<List<AttendancePerformanceDto>>
                .Failure(
                    "No employees found.");
        }


        // =========================================================
        // Attendance Records
        // =========================================================

        var records =
            await _context.AttendanceRecords
                .AsNoTracking()
                .Where(x =>
                    x.Date >= startDate &&
                    x.Date <= endDate)
                .ToListAsync(
                    cancellationToken);


        // =========================================================
        // Approved Leaves
        // =========================================================

        var leaves =
            await _context.EmployeeLeaves
                .AsNoTracking()
                .Where(x =>
                    x.Status == LeaveStatus.Approved &&
                    x.StartDate <= endDate &&
                    x.EndDate >= startDate)
                .ToListAsync(
                    cancellationToken);


        // =========================================================
        // Existing Performances
        // =========================================================

        var performances =
            await _context.AttendancePerformances
                .Where(x =>
                    x.Year == year &&
                    x.Month == month)
                .ToListAsync(
                    cancellationToken);


        // =========================================================
        // Result
        // =========================================================

        var result =
            new List<AttendancePerformanceDto>();


        // =========================================================
        // Calculate Every Employee
        // =========================================================

        foreach (var employee in employees)
        {
            var employeeRecords =
                records
                    .Where(x =>
                        x.EmployeeId == employee.Id)
                    .ToList();


            // إذا لم توجد سجلات حضور للموظف
            // لا نعتبرها خطأ على مستوى العملية كلها.
            if (employeeRecords.Count == 0)
            {
                continue;
            }


            var employeeLeaves =
                leaves
                    .Where(x =>
                        x.EmployeeId == employee.Id)
                    .ToList();


            // =====================================================
            // Calculate Performance
            // =====================================================

            var calculation =
                await _calculator.CalculateAsync(
                    employeeRecords,
                    employeeLeaves,
                    startDate,
                    endDate,
                    policy);


            // =====================================================
            // Get Existing Performance
            // =====================================================

            var performance =
                performances.FirstOrDefault(
                    x =>
                        x.EmployeeId == employee.Id);


            // =====================================================
            // Create Performance
            // =====================================================

            if (performance == null)
            {
                performance =
                    new AttendancePerformance
                    {
                        EmployeeId =
                            employee.Id,

                        Year =
                            year,

                        Month =
                            month
                    };

                await _context.AttendancePerformances
                    .AddAsync(
                        performance,
                        cancellationToken);

                performances.Add(performance);
            }


            // =====================================================
            // Attendance Score
            // =====================================================

            performance.AttendanceScore =
                calculation.FinalScore;


            // =====================================================
            // Attendance Statistics
            // =====================================================

            performance.PresentDays =
                calculation.PresentDays;

            performance.AbsentDays =
                calculation.AbsentDays;

            performance.PartialAttendanceDays =
                calculation.PartialAttendanceDays;

            performance.MissingCheckInCount =
                calculation.MissingCheckInCount;

            performance.MissingCheckOutCount =
                calculation.MissingCheckOutCount;


            // =====================================================
            // Leave Statistics
            // =====================================================

            performance.AnnualLeaveDays =
                calculation.AnnualLeaveDays;

            performance.SickLeaveDays =
                calculation.SickLeaveDays;

            performance.EmergencyLeaveDays =
                calculation.EmergencyLeaveDays;

            performance.UnpaidLeaveDays =
                calculation.UnpaidLeaveDays;


            // =====================================================
            // Time Statistics
            // =====================================================

            performance.TotalLateMinutes =
                calculation.TotalLateMinutes;

            performance.TotalEarlyLeaveMinutes =
                calculation.TotalEarlyLeaveMinutes;

            performance.EarlyLeaveDays =
                calculation.EarlyLeaveDays;

            performance.TotalLostTimeMinutes =
                calculation.TotalLostTimeMinutes;


            // =====================================================
            // Penalties
            // =====================================================

            performance.TotalPenaltyPoints =
                calculation.TotalPenaltyPoints;


            // =====================================================
            // Map Result
            // =====================================================

            result.Add(
                Map(
                    performance,
                    employee.Name));
        }


        // =========================================================
        // Save Once
        // =========================================================

        await _context.SaveChangesAsync(
            cancellationToken);


        // =========================================================
        // Return
        // =========================================================

        return Result<List<AttendancePerformanceDto>>
            .Succeeded(result);
    }




    private static (DateOnly Start, DateOnly End) GetMonthRange(
    int year,
    int month)
    {
        var start =
            new DateOnly(
                year,
                month,
                1);

        var end =
            start
            .AddMonths(1)
            .AddDays(-1);


        return (start, end);
    }


    private static AttendancePerformanceDto Map(
        AttendancePerformance performance,
        string employeeName)
    {
        return new AttendancePerformanceDto
        {
            Id = performance.Id,

            EmployeeId =
                performance.EmployeeId,

            EmployeeName =
                employeeName,

            Year =
                performance.Year,

            Month =
                performance.Month,

            AttendanceScore =
                performance.AttendanceScore,

            PresentDays =
                performance.PresentDays,

            AbsentDays =
                performance.AbsentDays,

            PartialAttendanceDays =
                performance.PartialAttendanceDays,

            EarlyLeaveDays =
          performance.EarlyLeaveDays,


            AnnualLeaveDays =
         performance.AnnualLeaveDays,


            SickLeaveDays =
         performance.SickLeaveDays,


            UnpaidLeaveDays =
        performance.UnpaidLeaveDays,

            MissingCheckInCount =
                performance.MissingCheckInCount,

            MissingCheckOutCount =
                performance.MissingCheckOutCount,

            TotalLateMinutes =
                performance.TotalLateMinutes,

            TotalLostTimeMinutes =
                performance.TotalLostTimeMinutes,

            TotalEarlyLeaveMinutes =
                performance.TotalEarlyLeaveMinutes,

            TotalPenaltyPoints =
                performance.TotalPenaltyPoints,

            CreatedOn =
                performance.CreatedOn
        };
    }

}