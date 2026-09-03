using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.EmployeeLoans.DTOs;

public class EmployeeLoanFilterDto
{
    // =========================================================
    // Loan Filters
    // =========================================================

    public int? EmployeeId { get; set; }

    public LoanStatus? Status { get; set; }

    public string? Title { get; set; }

    // سنة بداية القرض
    public int? StartYear { get; set; }

    // شهر بداية القرض
    public int? StartMonth { get; set; }


    // =========================================================
    // Installment Filters
    // =========================================================

    // سنة استحقاق القسط
    public int? InstallmentYear { get; set; }

    // شهر استحقاق القسط
    public int? InstallmentMonth { get; set; }

    public LoanInstallmentStatus? InstallmentStatus { get; set; }


    // =========================================================
    // Pagination
    // =========================================================

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}
