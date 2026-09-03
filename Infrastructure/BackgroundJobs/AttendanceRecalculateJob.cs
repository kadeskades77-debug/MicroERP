using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MicroERP.Infrastructure.BackgroundJobs;

public class AttendanceRecalculateJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _cache;


    public AttendanceRecalculateJob(
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
                var now = DateTime.Now;



                // التشغيل الساعة 1 ليلاً
                if (now.Hour == 1)
                {
                    var date =
                        DateOnly.FromDateTime(
                            now.AddDays(-1));



                    var cacheKey =
                        $"attendance-recalculate-{date:yyyyMMdd}";



                    if (!_cache.TryGetValue(
                        cacheKey,
                        out _))
                    {
                        using var scope =
                            _scopeFactory.CreateScope();



                        var service =
                            scope.ServiceProvider
                            .GetRequiredService<
                                IAttendanceRecalculateService>();



                        var result =
                            await service
                            .RecalculateDayAsync(
                                date,
                                stoppingToken);



                        if (result.Success)
                        {
                            _cache.Set(
                                cacheKey,
                                true,
                                TimeSpan.FromDays(1));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Attendance Recalculate Error: {ex.Message}");
            }



            await Task.Delay(
                TimeSpan.FromMinutes(30),
                stoppingToken);
        }
    }
}