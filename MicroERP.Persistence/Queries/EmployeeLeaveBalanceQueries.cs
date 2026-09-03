using MicroERP.Application.Features.Leaves.EmployeeLeaveBalances.Interfaces;
using MicroERP.Domin.Entities.EmployeeLeaves;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Queries
{
 
    public class EmployeeLeaveBalanceQueries : IEmployeeLeaveBalanceQueries
    {
        private readonly ApplicationDbContext _context;


        public EmployeeLeaveBalanceQueries(
            ApplicationDbContext context)
        {
            _context = context;
        }



        // Get all balances for employee in specific year
        public async Task<List<EmployeeLeaveBalance>> GetByEmployeeIdAsync(int employeeId,int year,
            CancellationToken cancellationToken = default)
        {
            return await _context.EmployeeLeaveBalances
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Year == year)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }



        // Get specific leave type balance
        public async Task<EmployeeLeaveBalance?> GetAsync(int employeeId,int year,LeaveType leaveType,
        CancellationToken cancellationToken = default)
        {
            return await _context.EmployeeLeaveBalances
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(
                    x =>
                        x.EmployeeId == employeeId &&
                        x.Year == year &&
                        x.LeaveType == leaveType,
                    cancellationToken);
        }
    }
}
