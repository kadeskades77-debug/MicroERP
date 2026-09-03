using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.DTOs;

namespace MicroERP.Application.Features.Payrolls.Interfaces;

public interface IPayrollQueries
{
    Task<Result<PagedResult<PayrollListDto>>> GetPagedAsync(
    PayrollFilterDto filter,
    CancellationToken cancellationToken);



    Task<Result<List<PayrollPeriodDto>>> GetPeriodsAsync(
        CancellationToken cancellationToken);

    Task<Result<PayrollPeriodDto?>> GetPeriodByIdAsync(int id,
        CancellationToken cancellationToken);



    Task<Result<List<PayrollListDto>>> GetByPeriodAsync(
        int periodId,
        CancellationToken cancellationToken);



    Task<Result<PayrollDetailsDto>> GetDetailsAsync(
        int payrollId,
        CancellationToken cancellationToken);



    Task<Result<List<PayrollListDto>>> GetByEmployeeAsync(int employeeId,
        CancellationToken cancellationToken);

    // يعرض ملخص فقط لكل موظف.
    //تجلب كل الرواتب التي تم دفعها:

    Task<Result<List<PayrollListDto>>> GetPaidPayrollsAsync(
        CancellationToken cancellationToken);

    //كشف راتب موظف واحد.
    Task<Result<PayslipDto>> GetPayslipAsync(int payrollId,
    CancellationToken cancellationToken);
}