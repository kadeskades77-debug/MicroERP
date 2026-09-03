namespace MicroERP.Application.Features.Payrolls.DTOs
{
    public class EmployeeSalaryComponentDto
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = null!;


        public int SalaryComponentId { get; set; }

        public string ComponentName { get; set; } = null!;


        public decimal Amount { get; set; }


        public bool IsActiveComponent { get; set; }


        public DateOnly EffectiveFrom { get; set; }

        public DateOnly? EffectiveTo { get; set; }
    }
}
