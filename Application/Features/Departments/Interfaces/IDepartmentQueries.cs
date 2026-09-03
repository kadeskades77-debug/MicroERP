using MicroERP.Application.Features.Departments.DTOs;
using MicroERP.Domin.Entities.Employees;

namespace MicroERP.Application.Features.Departments.Interfaces
{
    public interface IDepartmentQueries
    {
        Task<Department?> GetByIdAsync(int id);
        Task<List<DepartmentListDto>> GetAllAsync();

        Task<Department?> GetByCodeAsync(string code);

        Task<Department?> GetByNameArAsync(string nameAr);

        Task<bool> HasManager(int id);

        Task<Department?> GetByIdWithEmployeesAsync(int id);

        Task<Department?> GetByManagerIdAsync(int managerEmployeeId);

        Task<Department?> GetDeletedByIdAsync(int id);

        Task<bool> ExistsAsync(int id);

        Task<bool> CodeExistsAsync(string code);

        Task<bool> CodeExistsAsync(string code, int excludeDepartmentId);

        Task<bool> NameArExistsAsync(string nameAr, int excludeDepartmentId);
    }
}
