using MicroERP.Domin.Entities.Policies;
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces
{
    public interface IOvertimeCalculationService
    {
        Task<decimal> GetBasicSalaryAsync(
            int employeeId, CancellationToken cancellationToken);

        decimal GetMultiplier(
            OvertimePolicy policy,
            OvertimeType type);

        (decimal HourlyRate, decimal Amount)CalculateOvertimeAmount(
            int totalMinutes,
            decimal multiplier,
            decimal basicSalary,
            decimal totalWorkingMinutes);
    }
}
