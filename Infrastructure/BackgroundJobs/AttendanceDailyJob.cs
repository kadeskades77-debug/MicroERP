using MicroERP.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Interfaces;

namespace MicroERP.Infrastructure.BackgroundJobs;

public class AttendanceDailyJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _cache;


    public AttendanceDailyJob(
        IServiceScopeFactory scopeFactory,
        IMemoryCache cache)
    {
        _scopeFactory = scopeFactory;
        _cache = cache;
    }



    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope =
                    _scopeFactory.CreateScope();



                var context =
                    scope.ServiceProvider
                    .GetRequiredService<IApplicationDbContext>();



                var logProcessor =
                    scope.ServiceProvider
                    .GetRequiredService<IAttendanceLogProcessor>();



                var absentService =
                    scope.ServiceProvider
                    .GetRequiredService<AttendanceAbsentService>();



                var now = DateTime.Now;



                var lastShiftEnd =
                    await context.WorkSchedules
                        .Where(x => x.IsActive)
                        .Select(x =>
                            x.SecondShiftEnd ??
                            x.FirstShiftEnd)
                        .MaxAsync(
                            stoppingToken);



                var workEnd =
                    DateTime.Today
                    .Add(lastShiftEnd.ToTimeSpan());



                if (now >= workEnd)
                {
                    var cacheKey =
                        $"attendance-daily-{DateTime.Today:yyyyMMdd}";



                    if (!_cache.TryGetValue(cacheKey, out _))
                    {
                        // 1- معالجة بصمات الأجهزة
                        await logProcessor
                            .ProcessPendingLogsAsync(
                                stoppingToken);



                        // 2- إنشاء الغياب للموظفين بدون حضور
                        await absentService
                            .CreateAbsentRecordsAsync(
                                DateOnly.FromDateTime(now),
                                stoppingToken);



                        _cache.Set(
                            cacheKey,
                            true,
                            TimeSpan.FromDays(1));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    ex.Message);
            }



            await Task.Delay(
                TimeSpan.FromMinutes(30),
                stoppingToken);
        }
    }
}