using MicroERP.Application.Features.EmployeeLeaves.Interfaces;
using MicroERP.Domin.Entities;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Queries
{
    public class EmployeeLeaveQueries : IEmployeeLeaveQueries
    {
        private readonly ApplicationDbContext _context;

        public EmployeeLeaveQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmployeeLeave>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.EmployeeLeaves
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<EmployeeLeave?> GetByIdAsync(int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.EmployeeLeaves
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<EmployeeLeave>> GetByEmployeeIdAsync(int employeeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.EmployeeLeaves
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .Where(x => x.EmployeeId == employeeId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.EmployeeLeaves
                .AnyAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> HasOverlappingLeaveAsync(int employeeId,DateOnly startDate,DateOnly endDate,
       int? excludeLeaveId = null,
       CancellationToken cancellationToken = default)
        {
            var query = _context.EmployeeLeaves
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    (x.Status == LeaveStatus.Pending ||
                     x.Status == LeaveStatus.Approved));


            if (excludeLeaveId.HasValue)
            {
                query = query.Where(
                    x => x.Id != excludeLeaveId.Value);
            }


            return await query.AnyAsync(
                x =>
                    startDate <= x.EndDate &&
                    endDate >= x.StartDate,
                cancellationToken);
        }
    }
}
