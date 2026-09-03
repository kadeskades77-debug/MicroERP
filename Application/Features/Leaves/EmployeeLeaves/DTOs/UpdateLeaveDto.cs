using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs
{
    public class UpdateLeaveDto
    {
        public LeaveType? LeaveType { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Reason { get; set; }
    }
}
