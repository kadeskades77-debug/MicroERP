using MicroERP.Application.Features.EmployeeLeaves.DTOs;
using MicroERP.Domin.Entities;

namespace MicroERP.Application.Common.Mappings
{
    public static class EmployeeLeaveMappings
    {
        public static LeaveDto ToDto(this EmployeeLeave leave)
        {
            return new LeaveDto
            {
                Id = leave.Id,

                EmployeeId = leave.EmployeeId,

                EmployeeName = leave.Employee.User.FullName,


                LeaveTypeId = (int)leave.LeaveType,

                LeaveType = leave.LeaveType.ToString(),


                StartDate = leave.StartDate,

                EndDate = leave.EndDate,


                TotalDays = leave.TotalDays,


                SickDays = leave.SickDays,

                EmergencyDays = leave.EmergencyDays,

                UnpaidDays = leave.UnpaidDays,


                StatusId = (int)leave.Status,

                Status = leave.Status.ToString(),


                Reason = leave.Reason,


                RejectionReason = leave.RejectionReason,


                HasWarning = false,

                WarningMessage = null,


                ApprovedOn = leave.ApprovedOn,

                RejectedOn = leave.RejectedOn
            };
        }

        public static LeaveListDto ToListDto(this EmployeeLeave leave)
        {
            return new LeaveListDto
            {
                Id = leave.Id,

                EmployeeId = leave.EmployeeId,

                EmployeeName = leave.Employee.User.FullName,


                LeaveTypeId = (int)leave.LeaveType,

                LeaveType = leave.LeaveType.ToString(),


                StartDate = leave.StartDate,

                EndDate = leave.EndDate,


                TotalDays = leave.TotalDays,


                StatusId = (int)leave.Status,

                Status = leave.Status.ToString()
            };
        }
    }
}
