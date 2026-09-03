using MicroERP.Application.Features.Payrolls.Interfaces;
using MicroERP.Application.Features.Payrolls.SalaryComponents.Services;
using MicroERP.Domin.Entities.Payrolls;
using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.Payrolls.Services.Calculators;

public class PayrollCalculator : IPayrollCalculator
{
    public PayrollCalculationResult Calculate(
    IEnumerable<EmployeeSalaryComponent> components)
    {
        var componentList = components.ToList();

        var basicComponent =
            componentList.FirstOrDefault(x =>
                x.SalaryComponent.Code ==
                SalaryComponentCodes.Basic);

        var basicSalary =
            basicComponent?.Amount ?? 0;

        decimal allowances = 0;
        decimal deductions = 0;

        var items = new List<PayrollItem>();

        foreach (var component in componentList)
        {
            decimal amount;

            if (component.SalaryComponent.Code ==
                SalaryComponentCodes.Basic)
            {
                amount = component.Amount;
            }
            else
            {
                amount = CalculateAmount(
                    component,
                    basicSalary);
            }

            if (component.SalaryComponent.Code !=
                SalaryComponentCodes.Basic)
            {
                if (component.SalaryComponent.Type ==
                    SalaryComponentType.Allowance)
                {
                    allowances += amount;
                }
                else if (component.SalaryComponent.Type ==
                         SalaryComponentType.Deduction)
                {
                    deductions += amount;
                }
            }

            items.Add(new PayrollItem
            {
                SalaryComponentId =
                    component.SalaryComponentId,

                Amount = amount,

                ItemName =
                    component.SalaryComponent.NameAr,

                Description =
                    $"{component.SalaryComponent.Code} Component",

                Source =
                    PayrollItemSource.SalaryComponent,

                Type =
                    component.SalaryComponent.Type
            });
        }

        var grossSalary =
            basicSalary + allowances;

       

        return new PayrollCalculationResult
        {
            GrossSalary = grossSalary,

            TotalAllowances = allowances,

            TotalOvertime = 0,

            TotalDeductions = deductions,

            NetSalary = 0,

            Items = items
        };
    }


    private decimal CalculateAmount(
        EmployeeSalaryComponent component,
        decimal basicSalary)
    {
        if (component.SalaryComponent.CalculationType ==
            CalculationType.Percentage)
        {
            return basicSalary *
                   component.Amount /
                   100;
        }

        return component.Amount;
    }

}