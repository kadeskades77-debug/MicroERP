
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MicroERP.Infrastructure.BackgroundJobs;

public class AttendancePerformanceMonthlyJob
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;


    public AttendancePerformanceMonthlyJob(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
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


                var performanceService =
                    scope.ServiceProvider
                    .GetRequiredService<IAttendancePerformanceService>();

                var logProcessor =
                   scope.ServiceProvider
                   .GetRequiredService<IAttendanceLogProcessor>();




                var today = DateTime.Today;



                // يتم التنفيذ أول يوم من الشهر
                if (today.Day == 1)
                {
                    var previousMonth =
                        today.AddMonths(-1);



                    var year =
                        previousMonth.Year;


                    var month =
                        previousMonth.Month;



                    var employeeIds =
                        await context.Employees
                        .Where(x => x.IsActive)
                        .Select(x => x.Id)
                        .ToListAsync(
                            stoppingToken);



                    foreach (var employeeId in employeeIds)
                    {
                        await performanceService
                            .CalculateAsync(
                                employeeId,
                                year,
                                month,
                                stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    ex.Message);
            }



            await Task.Delay(
                TimeSpan.FromHours(12),
                stoppingToken);
        }
    }
}