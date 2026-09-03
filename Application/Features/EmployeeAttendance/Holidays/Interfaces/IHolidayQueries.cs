using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Holidays.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Holidays.Interfaces;

public interface IHolidayQueries
{
    Task<Result<HolidayDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default);


    Task<Result<PagedResult<HolidayDto>>> GetAllAsync(HolidayFilterDto filter,PagedRequest request,
        CancellationToken cancellationToken = default);


    Task<Result<List<HolidayDto>>> GetByDateAsync(DateOnly date,
        CancellationToken cancellationToken = default);
}