

using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Reports.DTOs
{
    public class BankTransferReportDto
    {
        public int PayrollPeriodId { get; set; }

        public string Period { get; set; } = null!;


        public int EmployeesCount { get; set; }


        public decimal TotalTransferAmount { get; set; }


        public List<BankTransferEmployeeDto> Employees { get; set; } = [];
    }



    public class BankTransferEmployeeDto
    {
        public int PayrollId { get; set; }


        public int EmployeeId { get; set; }


        public string EmployeeName { get; set; } = null!;


        public string? BankName { get; set; }


        public string? AccountNumber { get; set; }

        public string? IBAN { get; set; }


        public decimal Amount { get; set; }


        public PayrollStatus Status { get; set; }
    }
}
