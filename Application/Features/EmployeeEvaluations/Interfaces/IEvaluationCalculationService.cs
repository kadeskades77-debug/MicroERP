using MicroERP.Domin.Enums;

namespace MicroERP.Application.Features.EmployeeEvaluations.Interfaces;
public interface IEvaluationCalculationService 
{ 
    decimal CalculateTotalScore(IEnumerable<EmployeeEvaluationItemDto> items); 
    FinalRate CalculateFinalRate(decimal score); 
}
