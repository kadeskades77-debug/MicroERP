using AutoMapper;
using AutoMapper.QueryableExtensions;
using MicroERP.Application.Common.Extensions;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.Adjustments.DTOs;
using MicroERP.Application.Features.Payrolls.Adjustments.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Queries;

public class PayrollAdjustmentQueries
    : IPayrollAdjustmentQueries
{
    private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;


    public PayrollAdjustmentQueries(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }



    public async Task<Result<PagedResult<PayrollAdjustmentDto>>> GetAllAsync(
        PayrollAdjustmentFilterDto filter,
        PagedRequest request,
        CancellationToken cancellationToken)
    {
        var query =
            _context.PayrollAdjustments
                .AsNoTracking()
                .AsQueryable();


        // =========================================================
        // Filters
        // =========================================================

        if (filter.EmployeeId.HasValue)
        {
            query = query.Where(
                x => x.EmployeeId == filter.EmployeeId.Value);
        }

        if (filter.PayrollPeriodId.HasValue)
        {
            query = query.Where(
                x => x.PayrollPeriodId == filter.PayrollPeriodId.Value);
        }

        if (filter.Type.HasValue)
        {
            query = query.Where(
                x => x.Type == filter.Type.Value);
        }

        if (filter.IsApplied.HasValue)
        {
            query = query.Where(
                x => x.IsApplied == filter.IsApplied.Value);
        }


        // =========================================================
        // Total Count
        // =========================================================

        var totalCount =
            await query.CountAsync(
                cancellationToken);


        // =========================================================
        // Pagination
        // =========================================================

        var items =
            await query
                .OrderByDescending(x => x.Id)
                .Skip(
                    (request.PageNumber - 1) *
                    request.PageSize)
                .Take(request.PageSize)
                .ProjectTo<PayrollAdjustmentDto>(
                    _mapper.ConfigurationProvider)
                .ToListAsync(
                    cancellationToken);


        // =========================================================
        // Result
        // =========================================================

        var result =
            new PagedResult<PayrollAdjustmentDto>(
                items,
                totalCount,
                request.PageNumber,
                request.PageSize);


        return Result<PagedResult<PayrollAdjustmentDto>>
            .Succeeded(result);
    }




    public async Task<Result<PayrollAdjustmentDto>> GetByIdAsync(int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _context.PayrollAdjustments
                .AsNoTracking()
                .Where(x => x.Id == id)
                .ProjectTo<PayrollAdjustmentDto>(
                    _mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (result == null)
        {
            return Result<PayrollAdjustmentDto>
                .Failure(
                    "Payroll adjustment not found.");
        }

        return Result<PayrollAdjustmentDto>
            .Succeeded(result);
    }
}