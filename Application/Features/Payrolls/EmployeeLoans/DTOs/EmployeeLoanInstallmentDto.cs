using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.EmployeeLoans.DTOs;

public class EmployeeLoanInstallmentDto
{
    public int Id { get; set; }

    public int EmployeeLoanId { get; set; }

    public int InstallmentNumber { get; set; }

    public DateOnly DueDate { get; set; }

    public DateOnly? SkippedAt { get; set; }

    public string? SkipReason { get; set; }

    public decimal Amount { get; set; }

    public LoanInstallmentStatus Status { get; set; }

    public int? PayrollAdjustmentId { get; set; }

    public DateTime? PaidAt { get; set; }

    public string? Notes { get; set; }

    public int? RescheduledFromInstallmentId { get; set; }
}
