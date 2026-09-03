using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;

namespace MicroERP.Domin.Entities.Payrolls
{
    public class EmployeeBankAccount : BaseEntity
    {
        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;


        public string BankName { get; set; } = null!;


        public string? BranchName { get; set; }


        public string AccountNumber { get; set; } = null!;


        public string? IBAN { get; set; }


        public bool IsPrimary { get; set; }


        public bool IsActive { get; set; } = true;
    }
}
