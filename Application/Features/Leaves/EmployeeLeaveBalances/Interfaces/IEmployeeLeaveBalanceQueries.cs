using MicroERP.Domin.Entities.EmployeeLeaves;
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Interfaces;

public interface IEmployeeLeaveBalanceQueries
{
    Task<List<EmployeeLeaveBalance>> GetByEmployeeIdAsync(int employeeId,int year,
        CancellationToken cancellationToken = default);


    Task<EmployeeLeaveBalance?> GetAsync(int employeeId,int year,LeaveType leaveType,
        CancellationToken cancellationToken = default);
}