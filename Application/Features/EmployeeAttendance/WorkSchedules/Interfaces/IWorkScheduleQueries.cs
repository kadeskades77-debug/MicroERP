using MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.Interfaces
{
    public interface IWorkScheduleQueries
    {
        Task<WorkScheduleDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<List<WorkScheduleDto>> GetAllAsync(
            CancellationToken cancellationToken = default);
    }
}
