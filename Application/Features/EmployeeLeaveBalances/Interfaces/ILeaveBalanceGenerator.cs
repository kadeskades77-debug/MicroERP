namespace MicroERP.Application.Features.EmployeeLeaveBalances.Interfaces
{
    public interface ILeaveBalanceGenerator
    {
        Task GenerateForEmployeeAsync(
            int employeeId,
            int year,
            CancellationToken cancellationToken = default);


        Task GenerateForAllEmployeesAsync(
            int year,
            CancellationToken cancellationToken = default);

        Task GenerateForYearAsync(
    int year,
    CancellationToken cancellationToken = default);
    }
}
