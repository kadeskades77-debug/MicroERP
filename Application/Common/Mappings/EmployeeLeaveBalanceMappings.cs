using MicroERP.Application.Features.EmployeeLeaveBalances.DTOs;
using MicroERP.Domin.Entities;

namespace MicroERP.Application.Common.Mappings;

public static class EmployeeLeaveBalanceMappings
{
    public static LeaveBalanceDto ToDto(
        this EmployeeLeaveBalance balance)
    {
        return new LeaveBalanceDto
        {
            Id = balance.Id,

            EmployeeId = balance.EmployeeId,

            EmployeeName = balance.Employee.User.FullName,


            Year = balance.Year,


            LeaveTypeId = (int)balance.LeaveType,

            LeaveType = balance.LeaveType.ToString(),


            TotalDays = balance.TotalDays,

            UsedDays = balance.UsedDays,

            RemainingDays = balance.RemainingDays
        };
    }
}