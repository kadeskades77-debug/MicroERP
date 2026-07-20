using Domin.Entities;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Departments.DTOs;
using MicroERP.Application.Features.Departments.Interfaces;
using Microsoft.EntityFrameworkCore;
using MicroERP.Application.Common.Mappings;
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

    public async Task<List<DepartmentListDto>> GetAllAsync()
    {
        return await _context.Departments
                .Select(x => x.ToListDto())
                
                .ToListAsync();
    }

    public async Task<Department?> GetByCodeAsync(string code)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(x => x.Code == code);
    }
   public async Task<Department?> GetDeletedByIdAsync(int id)
    {
    return await _context.Departments
    .IgnoreQueryFilters()
    .FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<Department?> GetByNameArAsync(string nameAr)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(x => x.NameAr == nameAr);
    }

    public async Task<Department?> GetByIdWithEmployeesAsync(int id)
    {
        return await _context.Departments
            .Include(x => x.Employees)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Department?> GetByManagerIdAsync(int managerEmployeeId)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(x =>
                x.ManagerEmployeeId == managerEmployeeId);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Departments
            .AnyAsync(x => x.Id == id);
    }
    public async Task<bool> HasManager(int id)
    {
        return await _context.Departments
            .AnyAsync(x => x.Id == id);
    }

    public async Task<bool> CodeExistsAsync(string code)
    {
        return await _context.Departments
            .AnyAsync(x => x.Code == code);
    }

    public async Task<bool> CodeExistsAsync(string code,int excludeDepartmentId)
    {
        return await _context.Departments
            .AnyAsync(x =>
                x.Code == code &&
                x.Id != excludeDepartmentId);
    }

    public async Task<bool> NameArExistsAsync(string nameAr,int excludeDepartmentId)
    {
        return await _context.Departments
            .AnyAsync(x =>
                x.NameAr == nameAr &&
                x.Id != excludeDepartmentId);
    }
}