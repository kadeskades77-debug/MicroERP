namespace MicroERP.Application.Features.Payrolls.EmployeeLoans.DTOs;

public class CreateEmployeeLoanDto
{
    public int EmployeeId { get; set; }
    public string Title { get; set; } = null!;
    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal InstallmentAmount { get; set; }
    public DateOnly StartDate { get; set; }
}
