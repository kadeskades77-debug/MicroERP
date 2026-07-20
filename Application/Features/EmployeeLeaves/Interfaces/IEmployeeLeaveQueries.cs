using MicroERP.Domin.Entities;

namespace MicroERP.Application.Features.EmployeeLeaves.Interfaces
{
    public interface IEmployeeLeaveQueries
    {
        Task<List<EmployeeLeave>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<EmployeeLeave?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<List<EmployeeLeave>> GetByEmployeeIdAsync(
            int employeeId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<bool> HasOverlappingLeaveAsync(
            int employeeId,
            DateOnly startDate,
            DateOnly endDate,
            int? excludeLeaveId = null,
            CancellationToken cancellationToken = default);
    }
}
