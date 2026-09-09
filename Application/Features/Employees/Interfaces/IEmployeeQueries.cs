using MicroERP.Application.Features.Employees.DTOs;
using MicroERP.Domin.Entities.Employees;

namespace MicroERP.Application.Features.Employees.Interfaces
{
    public interface IEmployeeQueries
    {
        Task<Employee?> GetByIdAsync(int id);

        Task<Employee?> GetByIdWithDepartmentAsync(int id);

        Task<Employee?> GetByUserIdAsync(string userId);

        Task<Employee?> GetByIdWithUserAsync(int id);

        Task<Employee?> GetByIdWithUserAndDepartmentAsync(int id);

        Task<bool> ExistsAsync(int id);

        Task<bool> PhoneExistsAsync(string phone);

        Task<bool> PhoneExistsAsync(string phone, int excludeEmployeeId);
        Task<List<EmployeeListDto>> GetAllAsync();

        Task<List<EmployeeListDto>> GetDeletedAsync();

        Task<Employee?> GetDeletedByIdAsync(int id);
    }
}
