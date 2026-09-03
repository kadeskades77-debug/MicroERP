using Microsoft.EntityFrameworkCore;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Employees.Interfaces;
using MicroERP.Application.Features.Employees.DTOs;
using MicroERP.Application.Common.Mappings;
using MicroERP.Domin.Entities.Employees;
namespace MicroERP.Persistence.Queries;

public class EmployeeQueries : IEmployeeQueries
{
    private readonly IApplicationDbContext _context;

    public EmployeeQueries(IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<List<EmployeeListDto>> GetAllAsync()
    {
        var employees = await _context.Employees
            .Include(x => x.User)
            .Include(x => x.Department)
            .ToListAsync();

        return employees
            .Select(x => x.ToListDto())
            .ToList();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.Id == id);
    }


    public async Task<Employee?> GetDeletedByIdAsync(int id)
    {
        return await _context.Employees
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    public async Task<Employee?> GetByIdWithDepartmentAsync(int id)
    {
        return await _context.Employees
            .Include(x => x.Department)
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    public async Task<Employee?> GetByUserIdAsync(string userId)
    {
        return await _context.Employees
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }


    public async Task<Employee?> GetByIdWithUserAsync(int id)
    {
        return await _context.Employees
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    public async Task<Employee?> GetByIdWithUserAndDepartmentAsync(int id)
    {
        return await _context.Employees
            .Include(x => x.User)
            .Include(x => x.Department)
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Employees
            .AnyAsync(x => x.Id == id);
    }


    public async Task<bool> PhoneExistsAsync(string phone)
    {
        return await _context.Employees
            .AnyAsync(x => x.Phone == phone);
    }


    public async Task<bool> PhoneExistsAsync(
        string phone,
        int excludeEmployeeId)
    {
        return await _context.Employees
            .AnyAsync(x =>
                x.Phone == phone &&
                x.Id != excludeEmployeeId);
    }
}