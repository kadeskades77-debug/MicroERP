using MicroERP.Application.Features.Payrolls.Services.Calculators;
using MicroERP.Domin.Entities.Payrolls;

namespace MicroERP.Application.Features.Payrolls.Interfaces
{
    public interface IPayrollCalculator
    {
        PayrollCalculationResult Calculate(
            IEnumerable<EmployeeSalaryComponent> components);
    }
}
