

namespace MicroERP.Application.Features.Payrolls.Reports.DTOs
{
    public class PayrollStatisticsDto
    {
        public int PayrollPeriodId { get; set; }

        public string Period { get; set; } = null!;

        public int EmployeesCount { get; set; }

        public int DraftEmployeesCount { get; set; }

        public int CalculatedEmployeesCount { get; set; }

        public int PaidEmployeesCount { get; set; }

        public int ApprovedEmployeesCount { get; set; }

        public decimal TotalGrossSalary { get; set; }

        public decimal TotalAllowances { get; set; }

        public decimal TotalOvertime { get; set; }

        public decimal TotalDeductions { get; set; }

        public decimal TotalNetSalary { get; set; }

        public decimal AverageNetSalary { get; set; }

        public decimal HighestNetSalary { get; set; }

        public decimal LowestNetSalary { get; set; }
    }
}
