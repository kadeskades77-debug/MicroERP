using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Mappings;
using MicroERP.Application.Features.Employees.DTOs;
using MicroERP.Application.Features.Employees.Interfaces;
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;
namespace MicroERP.Persistence.Queries;

public class EmployeeQueries : IEmployeeQueries
{
    private readonly IApplicationDbContext _context;

    public EmployeeQueries(IApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<List<EmployeeListDto>> GetAllAsync(
     EmployeeFilterDto? filter = null)
    {
        IQueryable<Employee> query =
            _context.Employees
                .Include(x => x.User)
                .Include(x => x.Department);

        if (!string.IsNullOrWhiteSpace(filter?.Search))
        {
            var search = filter.Search.Trim();

            query = filter.SearchBy switch
            {
                EmployeeSearchBy.Name =>
                    query.Where(x =>
                        x.User.FullName.StartsWith(search)),

                EmployeeSearchBy.Email =>
                    query.Where(x =>
                        x.User.Email != null &&
                        x.User.Email.Contains(search)),

                EmployeeSearchBy.Username =>
                    query.Where(x =>
                        x.User.UserName != null &&
                        x.User.UserName.Contains(search)),

                EmployeeSearchBy.Phone =>
                    query.Where(x =>
                        x.Phone.Contains(search)),

                EmployeeSearchBy.Department =>
                    query.Where(x =>
                        x.Department.NameAr.Contains(search) ||
                        x.Department.NameEn.Contains(search)),

                _ => query
            };
        }

        var employees = await query.ToListAsync();

        return employees
            .Select(x => x.ToListDto())
            .ToList();
    }

    public async Task<List<EmployeeListDto>> GetDeletedAsync()
    {
        var employees = await _context.Employees
            .IgnoreQueryFilters()
            .Include(x => x.User)
            .Include(x => x.Department)
            .Where(x => x.IsDeleted)
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