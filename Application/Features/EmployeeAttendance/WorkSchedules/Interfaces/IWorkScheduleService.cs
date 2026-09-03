using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.WorkSchedules.Interfaces
{
    public interface IWorkScheduleService
    {
        Task<Result<WorkScheduleDto>> CreateAsync(
            CreateWorkScheduleDto dto,
            CancellationToken cancellationToken = default);

        Task<Result<WorkScheduleDto>> UpdateAsync(int id, UpdateWorkScheduleDto dto,
      CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}
