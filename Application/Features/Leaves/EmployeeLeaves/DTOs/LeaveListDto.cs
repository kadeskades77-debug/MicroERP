using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs
{
    public class LeaveListDto
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = null!;

        public int LeaveTypeId { get; set; }

        public string LeaveType { get; set; } = null!;

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public int TotalDays { get; set; }

        public int StatusId { get; set; }

        public string Status { get; set; } = null!;
    }
}
