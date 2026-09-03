namespace MicroERP.Application.Features.Payrolls.DTOs;

public class PayslipDto
{
    public int PayrollId { get; set; }


    public string EmployeeName { get; set; } = null!;


    public string Period { get; set; } = null!;


    public decimal GrossSalary { get; set; }


    public decimal TotalAllowances { get; set; }


    public decimal TotalDeductions { get; set; }


    public decimal NetSalary { get; set; }


    public string Status { get; set; } = null!;


    public string? ApprovedByUserName { get; set; }


    public DateTime? ApprovedOn { get; set; }


    public string? PaidByUserName { get; set; }


    public DateTime? PaidOn { get; set; }


    public List<PayslipItemDto> Items { get; set; }
        = new();
}