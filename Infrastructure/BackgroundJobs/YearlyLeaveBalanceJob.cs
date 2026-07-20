using MicroERP.Application.Features.EmployeeLeaveBalances.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MicroERP.Infrastructure.BackgroundJobs;

public class YearlyLeaveBalanceJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;


    public YearlyLeaveBalanceJob(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }



    protected override async Task ExecuteAsync(
     CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();

            var generator = scope.ServiceProvider
                .GetRequiredService<ILeaveBalanceGenerator>();

            await generator.GenerateForYearAsync(
                DateTime.UtcNow.Year,
                stoppingToken);

            await Task.Delay(
                TimeSpan.FromHours(24),
                stoppingToken);
        }
    }
}