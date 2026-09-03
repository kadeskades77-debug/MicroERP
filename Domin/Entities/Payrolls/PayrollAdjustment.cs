using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Enums;

public class PayrollAdjustment : BaseEntity
{
    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;


    public int PayrollPeriodId { get; set; }

    public PayrollPeriod PayrollPeriod { get; set; }


    public int? SalaryComponentId { get; set; }

    public SalaryComponent? SalaryComponent { get; set; }


    public AdjustmentType Type { get; set; }


    public string Title { get; set; } = null!;


    public string? Notes { get; set; }


    public decimal Amount { get; set; }


    public bool IsApplied { get; set; }
}