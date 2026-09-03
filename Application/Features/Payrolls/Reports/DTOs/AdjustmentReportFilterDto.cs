

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Reports.DTOs
{
    public class AdjustmentReportFilterDto
    {
        public int PayrollPeriodId { get; set; }

        public int? EmployeeId { get; set; }

        public AdjustmentType? Type { get; set; }

        public bool? IsApplied { get; set; }
    }
}
