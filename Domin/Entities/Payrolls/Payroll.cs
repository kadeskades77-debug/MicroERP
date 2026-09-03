using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Enums;
using MicroERP.Domin.Identity;

namespace MicroERP.Domin.Entities.Payrolls;

public class Payroll : BaseEntity
{
    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;


    public int PayrollPeriodId { get; set; }

    public PayrollPeriod PayrollPeriod { get; set; } = null!;


    public decimal GrossSalary { get; set; }

    public decimal TotalAllowances { get; set; }

    public decimal TotalOvertime { get; set; }

    public decimal TotalDeductions { get; set; }

    public decimal NetSalary { get; set; }


    // Approval
    public DateTime? ApprovedOn { get; set; }

    public string? ApprovedByUserId { get; set; }

    public ApplicationUser? ApprovedByUser { get; set; }



    // Payment
    public DateTime? PaidOn { get; set; }

    public string? PaidByUserId { get; set; }

    public ApplicationUser? PaidByUser { get; set; }

    public PayrollStatus Status { get; set; }
    // تفاصيل البنك
    public string? BankName { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? IBAN { get; set; }


    public ICollection<PayrollItem> PayrollItems { get; set; }
        = new List<PayrollItem>();
 
}