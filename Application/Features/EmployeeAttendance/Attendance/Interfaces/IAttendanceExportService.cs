using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;

public interface IAttendanceExcelExportService
{
    Task<Result<byte[]>> ExportAttendanceAsync(
        AttendanceFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<Result<byte[]>> ExportDailyAttendanceAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<Result<byte[]>> ExportEmployeeAttendanceAsync(
        AttendanceFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<Result<byte[]>>
      ExportPerformanceAsync(
          int year,
          int month,
          CancellationToken cancellationToken = default);


    Task<Result<byte[]>> ExportMyAttendanceAsync(
     AttendanceFilterDto filter,
     CancellationToken cancellationToken = default);

    Task<Result<byte[]>>
       ExportEmployeePerformanceAsync(
           int employeeId,
           int year,
           int month,
           CancellationToken cancellationToken = default);

    Task<Result<byte[]>>
      ExportMyPerformanceAsync(
          int year,
          int month,
          CancellationToken cancellationToken = default);
}