using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.Payrolls;

public class PayrollItem : BaseEntity
{
    public int PayrollId { get; set; }

    public Payroll Payroll { get; set; } = null!;



    // قد يكون العنصر ناتجاً عن Salary Component
    // أو Attendance
    // أو Payroll Adjustment
    public int? SalaryComponentId { get; set; }

    public SalaryComponent? SalaryComponent { get; set; }



    // اسم العنصر وقت إنشاء الراتب
    // حتى لو تغير اسم SalaryComponent لاحقاً
    public string ItemName { get; set; } = null!;



    // الوصف التفصيلي (اختياري)
    public string? Description { get; set; }



    // Allowance / Deduction
    public SalaryComponentType Type { get; set; }



    // قيمة العنصر
    public decimal Amount { get; set; }



    // مصدر العنصر
    public PayrollItemSource Source { get; set; }
}