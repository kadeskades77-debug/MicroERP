using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Departments.DTOs;
using MicroERP.Application.Features.Departments.Interfaces;
using Microsoft.EntityFrameworkCore;
using MicroERP.Domin.Entities.Employees;

namespace MicroERP.Persistence.Queries;

public class DepartmentQueries : IDepartmentQueries
{
    private readonly IApplicationDbContext _context;

    public DepartmentQueries(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Department?> GetByIdAsync(int id)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Department?> GetByIdWithEmployeesAsync(int id)
    {
        return await _context.Departments
            .Include(x => x.Employees)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<Department?> GetByManagerIdAsync(
    int managerEmployeeId)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(x =>
                x.ManagerEmployeeId == managerEmployeeId);
    }
}