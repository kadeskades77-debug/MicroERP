
using MicroERP.Domin.Common;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Enums;
using MicroERP.Domin.Identity;

namespace MicroERP.Domin.Entities.Payrolls
{
    public class EmployeeLoan : BaseEntity
    {
        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string? Notes { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal InstallmentAmount { get; set; }

        public int NumberOfInstallments { get; set; }

        public int PaidInstallments { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal RemainingAmount { get; set; }

        public DateOnly StartDate { get; set; }

        public LoanStatus Status { get; set; }


        // =========================================================
        // Cancellation
        // =========================================================

        public DateTime? CancelledAt { get; set; }

        public string? CancelledByUserId { get; set; }

        public ApplicationUser? CancelledByUser { get; set; }

        public string? CancellationReason { get; set; }

        public DateTime? SuspendedAt { get; set; }

        public string? SuspendedByUserId { get; set; }

        public ApplicationUser? SuspendedByUser { get; set; }

        public string? SuspensionReason { get; set; }


        public ICollection<EmployeeLoanInstallment> Installments
        {
            get;
            set;
        } = [];
    }
}
