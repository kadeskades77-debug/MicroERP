using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.EmployeeLoans.DTOs;

public class EmployeeLoanDto
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = null!;

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

    public string? CancelledByUserName { get; set; }

    public string? CancellationReason { get; set; }


    // =========================================================
    // Suspension
    // =========================================================

    public DateTime? SuspendedAt { get; set; }

    public string? SuspendedByUserId { get; set; }

    public string? SuspendedByUserName { get; set; }

    public string? SuspensionReason { get; set; }


    // =========================================================
    // Installments
    // =========================================================

    public List<EmployeeLoanInstallmentDto> Installments { get; set; }
        = [];
}
