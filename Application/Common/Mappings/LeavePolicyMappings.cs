using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.DTOs;
using MicroERP.Domin.Entities.Policies;

namespace MicroERP.Application.Common.Mappings;

public static class LeavePolicyMappings
{
    public static LeavePolicyDto ToDto(
        this LeavePolicy policy)
    {
        return new LeavePolicyDto
        {
            Id = policy.Id,

            LeaveTypeId = (int)policy.LeaveType,

            LeaveType = policy.LeaveType.ToString(),

            DaysPerMonth = policy.DaysPerMonth,

            MaximumDaysPerYear = policy.MaximumDaysPerYear
        };
    }
}