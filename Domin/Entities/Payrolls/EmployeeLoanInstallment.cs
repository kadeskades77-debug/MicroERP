

using MicroERP.Domin.Common;
using MicroERP.Domin.Enums;

namespace MicroERP.Domin.Entities.Payrolls
{
    public class EmployeeLoanInstallment : BaseEntity
    {
        public int EmployeeLoanId { get; set; }

        public EmployeeLoan EmployeeLoan { get; set; } = null!;


        public int InstallmentNumber { get; set; }


        public DateOnly DueDate { get; set; }

        public DateOnly? SkippedAt { get; set; }

        public string? SkipReason { get; set; }

        public decimal Amount { get; set; }


        public LoanInstallmentStatus Status { get; set; }


        public int? PayrollAdjustmentId { get; set; }

        public PayrollAdjustment? PayrollAdjustment { get; set; }


        public DateTime? PaidAt { get; set; }


        public string? Notes { get; set; }
        public int? RescheduledFromInstallmentId { get; set; }

        public EmployeeLoanInstallment? RescheduledFromInstallment { get; set; }

        public ICollection<EmployeeLoanInstallment> RescheduledInstallments
        {
            get;
            set;
        } = [];
    }
}
