using MicroERP.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MicroERP.Application.Common.Mappings;
using MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.DTOs;
using MicroERP.Application.Features.Leaves.EmployeeSpecialLeaves.Interfaces;
namespace MicroERP.Persistence.Queries;
public class EmployeeSpecialLeaveQueries : IEmployeeSpecialLeaveQueries
{
    private readonly IApplicationDbContext _context;


    public EmployeeSpecialLeaveQueries(
        IApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<SpecialLeaveDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var leave = await _context.EmployeeSpecialLeaves
     .Include(x => x.Employee)
         .ThenInclude(x => x.User)

     .Include(x => x.ApprovedByUser)

     .Include(x => x.RejectedByUser)

     .FirstOrDefaultAsync(
         x => x.Id == id,
         cancellationToken);

        return leave?.ToDto();
    }





    public async Task<List<SpecialLeaveDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var leaves = await _context.EmployeeSpecialLeaves
      .Include(x => x.Employee)
          .ThenInclude(x => x.User)

      .Include(x => x.ApprovedByUser)

      .Include(x => x.RejectedByUser)

      .ToListAsync(cancellationToken);

        return leaves.Select(x => x.ToDto()).ToList();
    }
}