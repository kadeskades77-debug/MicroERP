using AutoMapper;
using AutoMapper.QueryableExtensions;
using MicroERP.Application.Common.Extensions;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.DTOs;
using MicroERP.Application.Features.Payrolls.Interfaces;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Queries
{
    public class PayrollQueries : IPayrollQueries
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;


        public PayrollQueries(
            ApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }



        public async Task<Result<List<PayrollPeriodDto>>> GetPeriodsAsync(
            CancellationToken cancellationToken)
        {
            var data = await _context.PayrollPeriods
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ProjectTo<PayrollPeriodDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);


            return Result<List<PayrollPeriodDto>>.Succeeded(data);
        }

        public async Task<Result<PayrollPeriodDto?>> GetPeriodByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
        {
            var period =
                await _context.PayrollPeriods
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.Id == id,
                        cancellationToken);

            if (period == null)
            {
                return Result<PayrollPeriodDto>
                    .Failure("Payroll period not found.");
            }

            var result =
                new PayrollPeriodDto
                {
                    Id = period.Id,
                    Year = period.Year,
                    Month = period.Month,
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    Status = period.Status
                };

            return Result<PayrollPeriodDto>
                .Succeeded(result);
        }




        public async Task<Result<List<PayrollListDto>>> GetByPeriodAsync(
            int periodId,
            CancellationToken cancellationToken)
        {

            var exists = await _context.PayrollPeriods
                .AnyAsync(
                    x => x.Id == periodId,
                    cancellationToken);


            if (!exists)
                return Result<List<PayrollListDto>>
                    .Failure("Payroll period not found");



            var data = await _context.Payrolls
                .Where(x => x.PayrollPeriodId == periodId)
                .OrderBy(x => x.Employee.User.FullName)
                .ProjectTo<PayrollListDto>(
                    _mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);



            return Result<List<PayrollListDto>>
                .Succeeded(data);
        }




        public async Task<Result<PayrollDetailsDto>> GetDetailsAsync(
            int payrollId,
            CancellationToken cancellationToken)
        {

            var data = await _context.Payrolls
                .Where(x => x.Id == payrollId)
                .ProjectTo<PayrollDetailsDto>(
                    _mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);



            if (data == null)
            {
                return Result<PayrollDetailsDto>
                    .Failure("Payroll not found");
            }


            return Result<PayrollDetailsDto>
                .Succeeded(data);
        }



        public async Task<Result<List<PayrollListDto>>> GetByEmployeeAsync(int employeeId,
           CancellationToken cancellationToken)
        {
            var data = await _context.Payrolls
                .Where(x => x.EmployeeId == employeeId)
                .ProjectTo<PayrollListDto>(
                    _mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);


            return Result<List<PayrollListDto>>
                .Succeeded(data);
        }



        public async Task<Result<List<PayrollListDto>>> GetPaidPayrollsAsync(
           CancellationToken cancellationToken)
        {
            var data = await _context.Payrolls
                .Where(x =>
                    x.Status == PayrollStatus.Paid)
                .ProjectTo<PayrollListDto>(
                    _mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);


            return Result<List<PayrollListDto>>
                .Succeeded(data);
        }


        public async Task<Result<PayslipDto>> GetPayslipAsync(int payrollId,
       CancellationToken cancellationToken)
        {
            var data = await _context.Payrolls
                .Where(x => x.Id == payrollId)
                .ProjectTo<PayslipDto>(
                    _mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);



            if (data == null)
            {
                return Result<PayslipDto>
                    .Failure("Payroll not found");
            }



            return Result<PayslipDto>
                .Succeeded(data);
        }


        public async Task<Result<PagedResult<PayrollListDto>>> GetPagedAsync(
            PayrollFilterDto filter,
            CancellationToken cancellationToken)
        {
            var query = _context.Payrolls
                .AsQueryable();



            if (filter.PeriodId.HasValue)
            {
                query = query.Where(x =>
                    x.PayrollPeriodId == filter.PeriodId.Value);
            }



            if (filter.EmployeeId.HasValue)
            {
                query = query.Where(x =>
                    x.EmployeeId == filter.EmployeeId.Value);
            }



            if (filter.Status.HasValue)
            {
                query = query.Where(x =>
                    x.Status == filter.Status.Value);
            }



            var request = new PagedRequest
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };



            var result = await query
                .OrderByDescending(x => x.Id)
                .ProjectTo<PayrollListDto>(
                    _mapper.ConfigurationProvider)
                .ToPagedResultAsync(
                    request,
                    cancellationToken);



            return Result<PagedResult<PayrollListDto>>
                .Succeeded(result);
        }
    }
}
