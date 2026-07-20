using MicroERP.Application.Features.EmployeeSpecialLeaves.DTOs;
using MicroERP.Domin.Entities;


namespace MicroERP.Application.Common.Mappings;

public static class EmployeeSpecialLeaveMapping
{
    public static SpecialLeaveDto ToDto(
        this EmployeeSpecialLeave leave)
    {
        return new SpecialLeaveDto
        {
            Id = leave.Id,

            EmployeeId = leave.EmployeeId,

            EmployeeName = leave.Employee?.User?.FullName
                           ?? "Unknown",


            TypeId = (int)leave.Type,

            Type = leave.Type.ToString(),


            StartDate = leave.StartDate,

            EndDate = leave.EndDate,

            TotalDays = leave.TotalDays,


            StatusId = (int)leave.Status,

            Status = leave.Status.ToString(),


            Reason = leave.Reason,

          //  AttachmentPath = leave.AttachmentPath,


            RejectionReason = leave.RejectionReason,


            ApprovedByUserName =
                leave.ApprovedByUser?.FullName,


            RejectedByUserName =
                leave.RejectedByUser?.FullName,


            ApprovedOn = leave.ApprovedOn,

            RejectedOn = leave.RejectedOn
        };
    }
}