

using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Overtime;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces
{
    public interface IOvertimeExcelExportService
    {
      Task<byte[]> ExportOvertimeAsync(
       OvertimeReportFilterDto filter,
       CancellationToken cancellationToken = default);

      Task<byte[]> ExportEmployeeOvertimeAsync(
         OvertimeReportFilterDto filter,
          CancellationToken cancellationToken = default);

        Task<byte[]> ExportEmployeeOvertimeTypeAsync(
            OvertimeReportFilterDto filter,
           CancellationToken cancellationToken = default);

        Task<byte[]> ExportEmployeeOvertimeSourceAsync(
         OvertimeReportFilterDto filter,
        CancellationToken cancellationToken = default);

    }
}
