
using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.Payrolls
{
    public class SalaryComponent : BaseEntity
    {
        public string NameAr { get; set; } = null!;

        public string NameEn { get; set; } = null!;

        public string Code { get; set; } = null!;

        public SalaryComponentType Type { get; set; }

        public CalculationType CalculationType { get; set; }

        public bool IsTaxable { get; set; }

        public bool IsAttendanceRelated { get; set; }

        public bool IsDefault { get; set; }

        public ICollection<EmployeeSalaryComponent> EmployeeSalaryComponents { get; set; }
            = new List<EmployeeSalaryComponent>();
    }
}
