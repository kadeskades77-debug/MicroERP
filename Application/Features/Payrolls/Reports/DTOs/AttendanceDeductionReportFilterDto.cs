

namespace MicroERP.Application.Features.Payrolls.Reports.DTOs
{
    public class AttendanceDeductionReportFilterDto
    {
        public int PayrollPeriodId { get; set; }

        public int? DepartmentId { get; set; }

        public int? EmployeeId { get; set; }
    }
}
