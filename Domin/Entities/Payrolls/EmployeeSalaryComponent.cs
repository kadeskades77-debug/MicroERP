using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;

namespace MicroERP.Domin.Entities.Payrolls
{
    public class EmployeeSalaryComponent : BaseEntity
    {
        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;

        public DateOnly EffectiveFrom { get; set; }

        public DateOnly? EffectiveTo { get; set; }

        public int SalaryComponentId { get; set; }

        public SalaryComponent SalaryComponent { get; set; } = null!;


        /// <summary>
        /// Amount value:
        /// FixedAmount = actual amount
        /// Percentage = percentage value
        /// </summary>
        public decimal Amount { get; set; }
        public bool IsActiveComponent { get; set; } = true;
    }
}
