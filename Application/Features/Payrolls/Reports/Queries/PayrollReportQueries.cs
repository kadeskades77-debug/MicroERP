using AutoMapper;
using AutoMapper.QueryableExtensions;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Payrolls.Reports.DTOs;
using MicroERP.Application.Features.Payrolls.Reports.Interfaces;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Payrolls.Reports.Queries;

public class PayrollReportQueries
    : IPayrollReportQueries
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;


    public PayrollReportQueries(
     IApplicationDbContext context,
     IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }



    public async Task<Result<PayrollSummaryReportDto>> GetPayrollSummaryAsync(
      PayrollSummaryFilterDto filter,
      CancellationToken cancellationToken = default)
    {
        var query =
            _context.Payrolls
                .AsNoTracking()
                .Where(x =>
                    x.PayrollPeriodId == filter.PayrollPeriodId);

        if (filter.DepartmentId.HasValue)
        {
            query =
                query.Where(x =>
                    x.Employee.DepartmentId ==
                    filter.DepartmentId.Value);
        }

        if (filter.Status.HasValue)
        {
            query =
                query.Where(x =>
                    x.Status ==
                    filter.Status.Value);
        }

        var payrolls =
            await query
                .Select(x => new PayrollSummaryDto
                {
                    PayrollId =
                        x.Id,

                    EmployeeId =
                        x.EmployeeId,

                    EmployeeName =
                        x.Employee.User.FullName,

                    Department =
                        x.Employee.Department.NameEn,

                    GrossSalary =
                        x.GrossSalary,

                    TotalAllowances =
                        x.TotalAllowances,

                    TotalOvertime =
                        x.TotalOvertime,

                    TotalDeductions =
                        x.TotalDeductions,

                    NetSalary =
                        x.NetSalary,

                    Status =
                        x.Status
                })
                .OrderBy(x => x.Department)
                .ThenBy(x => x.EmployeeName)
                .ToListAsync(cancellationToken);

        if (payrolls.Count == 0)
        {
            return Result<PayrollSummaryReportDto>.Failure(
                "No payroll records found.");
        }

        var period =
            await _context.PayrollPeriods
                .AsNoTracking()
                .Where(x =>
                    x.Id == filter.PayrollPeriodId)
                .Select(x => new
                {
                    x.Id,
                    x.Year,
                    x.Month,
                    x.StartDate,
                    x.EndDate
                })
                .FirstOrDefaultAsync(cancellationToken);

        if (period == null)
        {
            return Result<PayrollSummaryReportDto>.Failure(
                "Payroll period was not found.");
        }

   

        var result =
      new PayrollSummaryReportDto
      {
          Year = period.Year,

          Month = period.Month,

          Payrolls = payrolls,

          EmployeesCount =
              payrolls
                  .Select(x => x.EmployeeId)
                  .Distinct()
                  .Count(),

          TotalGrossSalary =
              payrolls.Sum(x => x.GrossSalary),

          TotalAllowances =
              payrolls.Sum(x => x.TotalAllowances),

          TotalOvertime =
              payrolls.Sum(x => x.TotalOvertime),

          TotalDeductions =
              payrolls.Sum(x => x.TotalDeductions),

          TotalNetSalary =
              payrolls.Sum(x => x.NetSalary)
      };

        return Result<PayrollSummaryReportDto>.Succeeded(
            result);
    }


    public async Task<Result<PayrollDetailsDto>>
     GetPayrollDetailsAsync(
         PayrollDetailsFilterDto filter,
         CancellationToken cancellationToken = default)
    {
        var payroll =
            await _context.Payrolls
                .AsNoTracking()
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .Include(x => x.Employee.Department)
                .Include(x => x.PayrollPeriod)
                .Include(x => x.ApprovedByUser)
                .Include(x => x.PaidByUser)
                .Include(x => x.PayrollItems)
                    .ThenInclude(x => x.SalaryComponent)
                .FirstOrDefaultAsync(
                    x => x.Id == filter.PayrollId,
                    cancellationToken);

        if (payroll == null)
        {
            return Result<PayrollDetailsDto>
                .Failure(
                    "Payroll not found.");
        }

        var result =
            new PayrollDetailsDto
            {
                PayrollId =
                    payroll.Id,

                EmployeeId =
                    payroll.EmployeeId,

                EmployeeName =
                    payroll.Employee.User.FullName,

                Department =
                    payroll.Employee.Department.NameEn,

                Period =
                    $"{payroll.PayrollPeriod.Month}/{payroll.PayrollPeriod.Year}",

                Status =
                    payroll.Status,

                GrossSalary =
                    payroll.GrossSalary,

                TotalAllowances =
                    payroll.TotalAllowances,

                TotalOvertime =
                    payroll.TotalOvertime,

                TotalDeductions =
                    payroll.TotalDeductions,

                NetSalary =
                    payroll.NetSalary,

                ApprovedOn =
                    payroll.ApprovedOn,

                ApprovedByUserName =
                    payroll.ApprovedByUser?.FullName,

                PaidOn =
                    payroll.PaidOn,

                PaidByUserName =
                    payroll.PaidByUser?.FullName,

                BankName =
                    payroll.BankName,

                BankAccountNumber =
                    payroll.BankAccountNumber,

                IBAN =
                    payroll.IBAN,

                Items =
          payroll.PayrollItems?
        .Where(x => x.SalaryComponent != null)
        .OrderBy(x =>
            x.SalaryComponent!.Type)
        .ThenBy(x =>
            x.SalaryComponent!.NameEn)
        .Select(x =>
            new PayrollItemDetailsDto
            {
                SalaryComponentId =
                    x.SalaryComponentId ?? 0,

                ComponentName =
                    x.SalaryComponent!.NameEn,

                Type =
                    x.SalaryComponent.Type,

                Amount =
                    x.Amount
            })
        .ToList()
    ?? []
            };

        return Result<PayrollDetailsDto>
            .Succeeded(result);
    }



    public async Task<Result<List<EmployeePayrollHistoryDto>>>
     GetEmployeePayrollHistoryAsync(
         EmployeePayrollHistoryFilterDto filter,
         CancellationToken cancellationToken = default)
    {
        var employeeExists =
            await _context.Employees
                .AnyAsync(
                    x => x.Id == filter.EmployeeId,
                    cancellationToken);

        if (!employeeExists)
        {
            return Result<List<EmployeePayrollHistoryDto>>
                .Failure(
                    "Employee not found.");
        }

        var payrolls =
            await _context.Payrolls
                .AsNoTracking()
                .Where(x =>
                    x.EmployeeId ==
                    filter.EmployeeId)
                .OrderByDescending(
                    x => x.PayrollPeriod.Year)
                .ThenByDescending(
                    x => x.PayrollPeriod.Month)
                .Select(x =>
                    new EmployeePayrollHistoryDto
                    {
                        PayrollId =
                            x.Id,

                        PayrollPeriodId =
                            x.PayrollPeriodId,

                        EmployeeId =
                            x.EmployeeId,

                        EmployeeName =
                            x.Employee.User.FullName,

                        Department =
                            x.Employee.Department.NameEn,

                        HireDate =
                            x.Employee.HireDate,

                        Year =
                            x.PayrollPeriod.Year,

                        Month =
                            x.PayrollPeriod.Month,

                        GrossSalary =
                            x.GrossSalary,

                        TotalAllowances =
                            x.TotalAllowances,

                        TotalOvertime =
                            x.TotalOvertime,

                        TotalDeductions =
                            x.TotalDeductions,

                        NetSalary =
                            x.NetSalary,

                        Status =
                            x.Status,

                        CreatedOn =
                            x.CreatedOn
                    })
                .ToListAsync(
                    cancellationToken);

        return Result<List<EmployeePayrollHistoryDto>>
            .Succeeded(payrolls);
    }


    public async Task<Result<PayrollStatisticsDto>> GetPayrollStatisticsAsync(
      PayrollStatisticsFilterDto filter,
      CancellationToken cancellationToken = default)
    {
        var period =
            await _context.PayrollPeriods
                .AsNoTracking()
                .Where(x => x.Id == filter.PayrollPeriodId)
                .Select(x => new
                {
                    x.Id,
                    x.Year,
                    x.Month
                })
                .FirstOrDefaultAsync(cancellationToken);

        if (period == null)
        {
            return Result<PayrollStatisticsDto>
                .Failure("Payroll period not found.");
        }

        var query =
            _context.Payrolls
                .AsNoTracking()
                .Where(x =>
                    x.PayrollPeriodId ==
                    filter.PayrollPeriodId);

        if (filter.DepartmentId.HasValue)
        {
            query = query.Where(x =>
                x.Employee.DepartmentId ==
                filter.DepartmentId.Value);
        }

        var payrolls =
            await query
                .ToListAsync(cancellationToken);

        if (payrolls.Count == 0)
        {
            return Result<PayrollStatisticsDto>
                .Failure("No payroll records found.");
        }

        var overtimeQuery =
            _context.PayrollItems
                .AsNoTracking()
                .Where(x =>
                    x.Payroll.PayrollPeriodId ==
                    filter.PayrollPeriodId &&
                    x.Source ==
                    PayrollItemSource.Overtime);

        if (filter.DepartmentId.HasValue)
        {
            overtimeQuery =
                overtimeQuery.Where(x =>
                    x.Payroll.Employee.DepartmentId ==
                    filter.DepartmentId.Value);
        }


        var result =
            new PayrollStatisticsDto
            {
                PayrollPeriodId =
                    period.Id,

                Period =
                    $"{period.Month:00}/{period.Year}",

                EmployeesCount =
                    payrolls.Count,

                DraftEmployeesCount =
                    payrolls.Count(x =>
                        x.Status ==
                        PayrollStatus.Draft),

                CalculatedEmployeesCount =
                    payrolls.Count(x =>
                        x.Status ==
                        PayrollStatus.Calculated),

                ApprovedEmployeesCount =
                    payrolls.Count(x =>
                        x.Status ==
                        PayrollStatus.Approved),

                PaidEmployeesCount =
                    payrolls.Count(x =>
                        x.Status ==
                        PayrollStatus.Paid),

                TotalGrossSalary =
                    payrolls.Sum(x =>
                        x.GrossSalary),

                TotalAllowances =
                    payrolls.Sum(x =>
                        x.TotalAllowances),

                TotalOvertime =
                 payrolls.Sum(x =>
                     x.TotalOvertime),

                TotalDeductions =
                    payrolls.Sum(x =>
                        x.TotalDeductions),

                TotalNetSalary =
                    payrolls.Sum(x =>
                        x.NetSalary),

                AverageNetSalary =
                    payrolls.Average(x =>
                        x.NetSalary),

                HighestNetSalary =
                    payrolls.Max(x =>
                        x.NetSalary),

                LowestNetSalary =
                    payrolls.Min(x =>
                        x.NetSalary)
            };

        return Result<PayrollStatisticsDto>
            .Succeeded(result);
    }

    public async Task<Result<List<SalaryComponentReportDto>>>
    GetSalaryComponentReportAsync(
    SalaryComponentReportFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        var periodExists =
            await _context.PayrollPeriods
            .AnyAsync(
                x => x.Id == filter.PayrollPeriodId,
                cancellationToken);


        if (!periodExists)
        {
            return Result<List<SalaryComponentReportDto>>
                .Failure("Payroll period not found.");
        }



        var result =
    await _context.PayrollItems
        .AsNoTracking()
        .Where(x =>
            x.Payroll.PayrollPeriodId ==
                filter.PayrollPeriodId &&
            x.SalaryComponent != null)
        .GroupBy(x => new
        {
            x.SalaryComponentId,
            x.SalaryComponent!.NameAr,
            x.SalaryComponent.Type
        })
            .Select(x => new SalaryComponentReportDto
            {
                SalaryComponentId =
                    x.Key.SalaryComponentId ?? 0,

                ComponentName =
                    x.Key.NameAr,

                Type =
                    x.Key.Type,

                EmployeesCount =
                    x.Count(),

                TotalAmount =
                    x.Sum(i => i.Amount)
            })
            .OrderBy(x => x.Type)
            .ThenBy(x => x.ComponentName)
            .ToListAsync(cancellationToken);



        return Result<List<SalaryComponentReportDto>>
            .Succeeded(result);
    }

    public async Task<Result<List<AdjustmentReportDto>>>
    GetAdjustmentReportAsync(
    AdjustmentReportFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        var periodExists =
            await _context.PayrollPeriods
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == filter.PayrollPeriodId,
                cancellationToken);


        if (!periodExists)
        {
            return Result<List<AdjustmentReportDto>>
                .Failure(
                    "Payroll period not found.");
        }


        var query =
            _context.PayrollAdjustments
            .AsNoTracking()
            .Where(x =>
                x.PayrollPeriodId ==
                filter.PayrollPeriodId);


        if (filter.EmployeeId.HasValue)
        {
            query =
                query.Where(x =>
                    x.EmployeeId ==
                    filter.EmployeeId.Value);
        }


        if (filter.Type.HasValue)
        {
            query =
                query.Where(x =>
                    x.Type ==
                    filter.Type.Value);
        }


        if (filter.IsApplied.HasValue)
        {
            query =
                query.Where(x =>
                    x.IsApplied ==
                    filter.IsApplied.Value);
        }


        var result =
      await query
          .Select(x => new AdjustmentReportDto
          {
              Id = x.Id,

              EmployeeId = x.EmployeeId,

              EmployeeName =
                  x.Employee.User.FullName,

              Department =
                  x.Employee.Department.NameAr,

              SalaryComponentId =
                  x.SalaryComponentId,

              SalaryComponentName =
                  x.SalaryComponent != null
                      ? x.SalaryComponent.NameAr
                      : null,

              Type = x.Type,

              Title = x.Title,

              Amount = x.Amount,

              Notes = x.Notes,

              IsApplied = x.IsApplied,

              // Payroll Period
              Year =
                  x.PayrollPeriod.Year,

              Month =
                  x.PayrollPeriod.Month
          })
          .OrderBy(x => x.EmployeeName)
          .ThenBy(x => x.Id)
          .ToListAsync(cancellationToken);


        if (!result.Any())
        {
            return Result<List<AdjustmentReportDto>>
                .Failure(
                    "No adjustment records found.");
        }


        return Result<List<AdjustmentReportDto>>
            .Succeeded(result);
    }


    public async Task<Result<List<AttendanceDeductionReportDto>>>
    GetAttendanceDeductionReportAsync(
    AttendanceDeductionReportFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        var period =
            await _context.PayrollPeriods
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == filter.PayrollPeriodId,
                cancellationToken);


        if (period == null)
        {
            return Result<List<AttendanceDeductionReportDto>>
                .Failure(
                    "Payroll period not found.");
        }



        var recordsQuery =
            _context.AttendanceRecords
            .AsNoTracking()
            .Where(x =>
                x.Date >= period.StartDate &&
                x.Date <= period.EndDate);



        if (filter.EmployeeId.HasValue)
        {
            recordsQuery =
                recordsQuery.Where(x =>
                    x.EmployeeId ==
                    filter.EmployeeId.Value);
        }



        if (filter.DepartmentId.HasValue)
        {
            recordsQuery =
                recordsQuery.Where(x =>
                    x.Employee.DepartmentId ==
                    filter.DepartmentId.Value);
        }



        var attendanceData =
            await recordsQuery
            .GroupBy(x => new
            {
                x.EmployeeId,
                x.Employee.User.FullName,
                Department =
                    x.Employee.Department.NameAr
            })
            .Select(x => new
            {
                x.Key.EmployeeId,

                EmployeeName =
                    x.Key.FullName,

                Department =
                    x.Key.Department,


                AbsentDays =
                    x.Count(r =>
                        r.Status ==
                        AttendanceStatus.Absent),


                LateMinutes =
                    x.Sum(r =>
                        r.LateMinutes),


                EarlyLeaveMinutes =
                    x.Sum(r =>
                        r.EarlyLeaveMinutes),


                LostTimeMinutes =
                    x.Sum(r =>
                        r.LostTimeMinutes)
            })
            .ToListAsync(cancellationToken);



        var deductionItems =
            await _context.PayrollItems
            .AsNoTracking()
            .Where(x =>
                x.Payroll.PayrollPeriodId ==
                filter.PayrollPeriodId &&
                x.Source ==
                PayrollItemSource.Attendance)
            .GroupBy(x =>
                x.Payroll.EmployeeId)
            .Select(x => new
            {
                EmployeeId =
                    x.Key,

                Amount =
                    x.Sum(i =>
                        i.Amount)
            })
            .ToListAsync(cancellationToken);



        var result =
            attendanceData
            .Select(x => new AttendanceDeductionReportDto
            {
                EmployeeId =
                    x.EmployeeId,

                EmployeeName =
                    x.EmployeeName,

                Department =
                    x.Department,


                AbsentDays =
                    x.AbsentDays,


                LateMinutes =
                    x.LateMinutes,


                EarlyLeaveMinutes =
                    x.EarlyLeaveMinutes,


                LostTimeMinutes =
                    x.LostTimeMinutes,


                DeductionAmount =
                    deductionItems
                    .FirstOrDefault(d =>
                        d.EmployeeId ==
                        x.EmployeeId)
                    ?.Amount ?? 0
            })
            .Where(x =>
                x.DeductionAmount > 0 ||
                x.AbsentDays > 0 ||
                x.LateMinutes > 0 ||
                x.EarlyLeaveMinutes > 0)
            .ToList();



        return Result<List<AttendanceDeductionReportDto>>
            .Succeeded(result);
    }

    public async Task<Result<BankTransferReportDto>>
     GetBankTransferReportAsync(
     BankTransferReportFilterDto filter,
     CancellationToken cancellationToken = default)
    {
        var period =
            await _context.PayrollPeriods
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == filter.PayrollPeriodId,
                cancellationToken);


        if (period == null)
        {
            return Result<BankTransferReportDto>
                .Failure(
                    "Payroll period not found.");
        }


        var query =
            _context.Payrolls
            .AsNoTracking()
            .Where(x =>
                x.PayrollPeriodId ==
                filter.PayrollPeriodId);


        if (filter.DepartmentId.HasValue)
        {
            query =
                query.Where(x =>
                    x.Employee.DepartmentId ==
                    filter.DepartmentId.Value);
        }


        if (filter.Status.HasValue)
        {
            query =
                query.Where(x =>
                    x.Status ==
                    filter.Status.Value);
        }


        var employees =
            await query
            .Select(x => new BankTransferEmployeeDto
            {
                PayrollId =
                    x.Id,


                EmployeeId =
                    x.EmployeeId,


                EmployeeName =
                    x.Employee.User.FullName,


                BankName =
                    x.BankName,


                AccountNumber =
                    x.BankAccountNumber,


                IBAN =
                    x.IBAN,


                Amount =
                    x.NetSalary,


                Status =
                    x.Status
            })
            .OrderBy(x =>
                x.EmployeeName)
            .ToListAsync(
                cancellationToken);


        if (!employees.Any())
        {
            return Result<BankTransferReportDto>
                .Failure(
                    "No payroll records found.");
        }


        var result =
            new BankTransferReportDto
            {
                PayrollPeriodId =
                    period.Id,


                Period =
                    $"{period.Month}/{period.Year}",


                EmployeesCount =
                    employees.Count,


                TotalTransferAmount =
                    employees.Sum(x =>
                        x.Amount),


                Employees =
                    employees
            };


        return Result<BankTransferReportDto>
            .Succeeded(result);
    }

    public async Task<Result<EmployeePayslipDto>> GetEmployeePayslipAsync(
        int payrollId,
        CancellationToken cancellationToken = default)
    {
        var payslip =
            await _context.Payrolls
                .AsNoTracking()
                .Where(x => x.Id == payrollId)
                .Select(x => new EmployeePayslipDto
                {
                    PayrollId = x.Id,

                    EmployeeId = x.EmployeeId,

                    EmployeeName =
                        x.Employee.User.FullName,

                    Department =
                        x.Employee.Department.NameEn,

                    Year =
                        x.PayrollPeriod.Year,

                    Month =
                        x.PayrollPeriod.Month,

                    StartDate =
                        x.PayrollPeriod.StartDate,

                    EndDate =
                        x.PayrollPeriod.EndDate,

                    GrossSalary =
                        x.GrossSalary,

                    TotalAllowances =
                        x.TotalAllowances,

                    TotalOvertime =
                        x.TotalOvertime,

                    TotalDeductions =
                        x.TotalDeductions,

                    NetSalary =
                        x.NetSalary,

                    Status =
                        x.Status,

                    ApprovedOn =
                        x.ApprovedOn,

                    PaidOn =
                        x.PaidOn,

                    BankName =
                        x.BankName,

                    BankAccountNumber =
                        x.BankAccountNumber,

                    IBAN =
                        x.IBAN,

                    Items =
               x.PayrollItems
                .OrderBy(i =>
                    i.Type == SalaryComponentType.Basic ? 1 :
                    i.Source == PayrollItemSource.Overtime ? 3 :
                    i.Type == SalaryComponentType.Allowance ? 2 :
                    i.Type == SalaryComponentType.Deduction ? 4 :
                    5)
                .ThenBy(i => i.ItemName)
                .Select(i => new EmployeePayslipItemDto
                {
                    Id = i.Id,
              
                    SalaryComponentId =
                        i.SalaryComponentId,

                   ItemName =
                   i.SalaryComponent != null &&
                   !string.IsNullOrWhiteSpace(i.SalaryComponent.NameEn)
                       ? i.SalaryComponent.NameEn
                       : i.ItemName,
                  
                    Description =
                        i.Description,
              
                    Type =
                        i.Type,
              
                    Amount =
                        i.Amount,
              
                    Source =
                        i.Source
                })
                .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

        if (payslip == null)
        {
            return Result<EmployeePayslipDto>.Failure(
                "Payroll was not found.");
        }

        return Result<EmployeePayslipDto>.Succeeded(
            payslip);
    }
}