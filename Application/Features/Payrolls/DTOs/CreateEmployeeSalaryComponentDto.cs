namespace MicroERP.Application.Features.Payrolls.DTOs
{
    public class CreateEmployeeSalaryComponentDto
    {
        public int EmployeeId { get; set; }

        public int SalaryComponentId { get; set; }

        public decimal Amount { get; set; }

        public DateOnly EffectiveFrom { get; set; }

        public DateOnly? EffectiveTo { get; set; }
    }
}
