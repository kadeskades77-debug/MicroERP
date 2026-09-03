namespace MicroERP.Application.Features.Payrolls.DTOs;

public class PayslipItemDto
{
    public string ComponentName { get; set; } = null!;


    public decimal Amount { get; set; }


    public int Type { get; set; }

}