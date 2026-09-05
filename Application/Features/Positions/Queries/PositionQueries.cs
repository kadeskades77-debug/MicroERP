using Microsoft.EntityFrameworkCore;
using MicroERP.Application.Features.Positions.Interfaces;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Domin.Entities.Employees;

namespace MicroERP.Application.Features.Positions.Queries;

public class PositionQueries : IPositionQueries
{
    private readonly IApplicationDbContext _context;

    public PositionQueries(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Position?> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Positions
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Position>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Positions
            .AsNoTracking()
            .OrderBy(x => x.NameEn)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(
        string code,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await _context.Positions
            .AnyAsync(
                x => x.Code == code &&
                     (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
    }

    public async Task<bool> HasEmployeesAsync(
        int positionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AnyAsync(x => x.PositionId == positionId, cancellationToken);
    }

    public async Task<bool> EmployeeExistsAsync(
    int employeeId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AnyAsync(
                x => x.Id == employeeId,
                cancellationToken);
    }

    public async Task<bool> EmployeeAssignedToPositionAsync(
        int employeeId,
        int positionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AnyAsync(
                x =>
                    x.Id == employeeId &&
                    x.PositionId == positionId,
                cancellationToken);
    }

    public async Task<List<Employee>> GetEmployeesAsync(
        int positionId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Position)
            .Where(x => x.PositionId == positionId)
            .OrderBy(x => x.User.UserName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> PositionAlreadyAssignedInDepartmentAsync(
    int positionId,
    int departmentId,
    int employeeId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AnyAsync(
                x =>
                    x.PositionId == positionId &&
                    x.DepartmentId == departmentId &&
                    x.Id != employeeId,
                cancellationToken);
    }

    public async Task<bool> PositionAlreadyAssignedCompanyWideAsync(
    int positionId,
    int employeeId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AnyAsync(
                x =>
                    x.PositionId == positionId &&
                    x.Id != employeeId,
                cancellationToken);
    }
}
