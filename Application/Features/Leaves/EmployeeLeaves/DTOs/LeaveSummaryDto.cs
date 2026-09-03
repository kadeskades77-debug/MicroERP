namespace MicroERP.Application.Features.Leaves.EmployeeLeaves.DTOs
{
    public class LeaveSummaryDto
    {
        public int TotalRequests { get; set; }

        public int Approved { get; set; }

        public int Pending { get; set; }

        public int Rejected { get; set; }

        public int Cancelled { get; set; }

        public int TotalDays { get; set; }

        public int SickDays { get; set; }

        public int EmergencyDays { get; set; }

        public int AnnualDays { get; set; }

        public int UnpaidDays { get; set; }
    }
}
