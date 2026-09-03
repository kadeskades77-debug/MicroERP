

using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces
{
    public interface IOvertimeReportQueries
    {
        Task<Result<OvertimeReportResultDto>>
      GetOvertimeReportAsync(
          OvertimeReportFilterDto filter,
          CancellationToken cancellationToken = default);

        Task<Result<EmployeeOvertimeReportDto>>
          GetEmployeeOvertimeReportAsync(
          OvertimeReportFilterDto filter,
          CancellationToken cancellationToken = default);

        Task<Result<EmployeeOvertimeReportDto>>
         GetEmployeeOvertimeTypeReportAsync(
         OvertimeReportFilterDto filter,
         CancellationToken cancellationToken = default);

        Task<Result<EmployeeOvertimeReportDto>>
         GetEmployeeOvertimeSourceReportAsync(
         OvertimeReportFilterDto filter,
         CancellationToken cancellationToken = default);
    }
}
