using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs
{
    public class LeaveReportFilterDto
    {
        public int? EmployeeId { get; set; }

        public int? DepartmentId { get; set; }

        public LeaveType? LeaveType { get; set; }

        public LeaveStatus? Status { get; set; }

        public int? Year { get; set; }

        public DateOnly? FromDate { get; set; }

        public DateOnly? ToDate { get; set; }
    }
}
