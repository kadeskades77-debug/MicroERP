using AutoMapper;
using AutoMapper.QueryableExtensions;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.SalaryComponents.DTOs;
using MicroERP.Application.Features.Payrolls.SalaryComponents.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Queries;

public class SalaryComponentQueries : ISalaryComponentQueries
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public SalaryComponentQueries(
        ApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<SalaryComponentDto>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var data = await _context.SalaryComponents
            .OrderBy(x => x.Type)
            .ThenBy(x => x.NameAr)
            .ProjectTo<SalaryComponentDto>(
                _mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<List<SalaryComponentDto>>
            .Succeeded(data);
    }

    public async Task<Result<SalaryComponentDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken)
    {
        var data = await _context.SalaryComponents
            .Where(x => x.Id == id)
            .ProjectTo<SalaryComponentDto>(
                _mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (data == null)
        {
            return Result<SalaryComponentDto>
                .Failure("Salary component not found");
        }

        return Result<SalaryComponentDto>
            .Succeeded(data);
    }
}