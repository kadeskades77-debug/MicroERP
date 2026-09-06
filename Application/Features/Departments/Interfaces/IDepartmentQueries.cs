using MicroERP.Application.Features.Departments.DTOs;
using MicroERP.Domin.Entities.Employees;

namespace MicroERP.Application.Features.Departments.Interfaces
{
    public interface IDepartmentQueries
    {
        Task<Department?> GetByIdAsync(int id);
    

        Task<Department?> GetByIdWithEmployeesAsync(int id);

        Task<Department?> GetByManagerIdAsync(int managerEmployeeId);

       
    }
}
