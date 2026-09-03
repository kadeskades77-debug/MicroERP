using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Domin.Entities.Policies;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services
{
    public class OvertimeCalculationService : IOvertimeCalculationService
    {
        private readonly IApplicationDbContext _context;

        public OvertimeCalculationService(IApplicationDbContext context)
        {
            _context = context;
        }

        public (decimal HourlyRate, decimal Amount) CalculateOvertimeAmount(
          int totalMinutes,
          decimal multiplier,
          decimal basicSalary,
          decimal totalWorkingMinutes)
        {
            if (totalMinutes <= 0)
            {
                throw new ArgumentException(
                    "Total minutes must be greater than zero.",
                    nameof(totalMinutes));
            }

            if (multiplier <= 0)
            {
                throw new ArgumentException(
                    "Multiplier must be greater than zero.",
                    nameof(multiplier));
            }

            if (basicSalary <= 0)
            {
                throw new ArgumentException(
                    "Basic salary must be greater than zero.",
                    nameof(basicSalary));
            }

            if (totalWorkingMinutes <= 0)
            {
                throw new ArgumentException(
                    "Total working minutes must be greater than zero.",
                    nameof(totalWorkingMinutes));
            }


            var minuteRate =
                basicSalary /
                totalWorkingMinutes;


            var hourlyRate =
                minuteRate * 60;


            var amount =
                totalMinutes *
                minuteRate *
                multiplier;


            return (

                HourlyRate:
                    decimal.Round(
                        hourlyRate,
                        2,
                        MidpointRounding.AwayFromZero),

        Amount:
            decimal.Round(
                amount,
                2,
                MidpointRounding.AwayFromZero)
            );
            
        }


        public async Task<decimal> GetBasicSalaryAsync(
        int employeeId,
        CancellationToken cancellationToken)
        {
            var basicSalary =
                await _context.EmployeeSalaryComponents
                    .Where(x =>
                        x.EmployeeId == employeeId &&
                        x.SalaryComponent.Code == "BASIC")
                    .Select(x => x.Amount)
                    .FirstOrDefaultAsync(cancellationToken);


            return basicSalary;
        }

        public decimal GetMultiplier(OvertimePolicy policy, OvertimeType type)
        {
            return type switch
            {
                OvertimeType.Weekend =>
                    policy.WeekendMultiplier,

                OvertimeType.Holiday =>
                    policy.HolidayMultiplier,

                OvertimeType.Normal =>
                    policy.NormalMultiplier,

                _ =>
                    throw new ArgumentOutOfRangeException(
                        nameof(type),
                        type,
                        "Unsupported overtime type.")
            };
        }
    }
}
