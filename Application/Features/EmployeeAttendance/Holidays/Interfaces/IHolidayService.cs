using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Holidays.DTOs;

namespace MicroERP.Application.Features.EmployeeAttendance.Holidays.Interfaces;

public interface IHolidayService
{
    Task<Result<HolidayDto>> CreateAsync(CreateHolidayDto dto,
        CancellationToken cancellationToken = default);


    Task<Result<HolidayDto>> UpdateAsync(int id,UpdateHolidayDto dto,
        CancellationToken cancellationToken = default);


    Task<Result<bool>> DeleteAsync(int id,
        CancellationToken cancellationToken = default);
}