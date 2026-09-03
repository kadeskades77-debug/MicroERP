using MicroERP.Application.Common.Excel.Interfaces;
using MicroERP.Application.Common.Excel.Models;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs.Report;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Entities.EmployeeLeaves;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;

public class AttendanceExcelExportService
    : IAttendanceExcelExportService
{
    private readonly IApplicationDbContext _context;
    private readonly IExcelExportService _excelExporter;
    private readonly ICurrentUserService _currentUserService;
    public AttendanceExcelExportService(
        IApplicationDbContext context, IExcelExportService excelExporter, ICurrentUserService currentUserService)
    {
        _context = context;
        _excelExporter = excelExporter;
        _currentUserService = currentUserService;
    }

    public async Task<Result<byte[]>> ExportDailyAttendanceAsync(
    DateOnly date,
    CancellationToken cancellationToken = default)
    {
        try
        {
            var result =
                await GetDailyAttendanceRecordsAsync(
                    date,
                    cancellationToken);

            if (!result.Success)
            {
                return Result<byte[]>
                    .Failure(result.Message);
            }

            var rows =
                result.Data!
                    .Select(CreateDailyAttendanceRow)
                    .ToList();

            var sheet =
                CreateSheet(
                    "Daily Attendance",
                    DailyAttendanceHeaders,
                    rows);

            var file =
                _excelExporter.Export(sheet);

            return Result<byte[]>
                .Succeeded(file);
        }
        catch (Exception ex)
        {
            return Result<byte[]>
                .Failure(
                    $"Failed to export daily attendance: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>> ExportAttendanceAsync(
     AttendanceFilterDto filter,
     CancellationToken cancellationToken = default)
    {
        try
        {
            var query =
                _context.AttendanceRecords
                .AsNoTracking()
                .Include(x => x.Employee)
                    .ThenInclude(e => e.User)
                .Include(x => x.Employee)
                    .ThenInclude(e => e.Department)
                .Include(x => x.Transactions)
                .AsQueryable();


            query =
                ApplyAttendanceFilter(
                    query,
                    filter);


            var attendanceRecords =
                await query
                .OrderByDescending(x => x.Date)
                .ToListAsync(cancellationToken);


            var records =
                attendanceRecords
                .Select(x => new AttendanceExportDto
                {
                    EmployeeId =
                        x.EmployeeId,

                    Employee =
                        x.Employee.User.FullName,

                    Department =
                        x.Employee.Department.NameAr,

                    Date =
                        x.Date,

                    CheckIn =
                        GetCheckIn(x.Transactions),

                    CheckOut =
                        GetCheckOut(x.Transactions),

                    ExpectedMinutes =
                        x.ExpectedMinutes,

                    WorkedMinutes =
                        x.WorkedMinutes,

                    LateMinutes =
                        x.LateMinutes,

                    LostTimeMinutes =
                        x.LostTimeMinutes,

                    EarlyLeaveMinutes =
                        x.EarlyLeaveMinutes,

                    Status =
                        x.Status,

                    Notes =
                        x.Notes
                })
                .ToList();

           
            var leaves =
                await GetAttendanceLeavesAsync(
                    filter,
                    cancellationToken);


            var specialLeaves =
                await GetAttendanceSpecialLeavesAsync(
                    filter,
                    cancellationToken);
            if (!specialLeaves.Success)
            {
                return Result<byte[]>
                    .Failure(specialLeaves.Message);
            }
         
            var specialLeaveLookup =
            BuildSpecialLeaveLookup(specialLeaves.Data);


            var leaveLookup =
                BuildLeaveLookup(leaves);


        



            var rows =
                records
                .Select(x =>
                    CreateAttendanceExportRow(
                        x,
                        leaveLookup,
                        specialLeaveLookup))
                .ToList();



            var sheet =
                CreateSheet(
                    "Attendance Records",
                    AttendanceHeaders,
                    rows);



            // =========================================================
            // Export
            // =========================================================

            var file =
                _excelExporter.Export(sheet);


            // =========================================================
            // Success
            // =========================================================

            return Result<byte[]>
                .Succeeded(file);
        }
        catch (Exception ex)
        {
            return Result<byte[]>
                .Failure(
                    $"Failed to export daily attendance: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>> ExportEmployeeAttendanceAsync(
    AttendanceFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        try { 
        var (from, to) =
            GetDateRange(filter);



        filter.DateFrom = from;
        filter.DateTo = to;



        var records =
            await GetAttendanceRecordsAsync(
                filter,
                cancellationToken);

            if (!records.Success)
            {
                return Result<byte[]>
                    .Failure(records.Message);
            }
           

            var leaves =
            await GetEmployeeLeavesAsync(
                filter,
                cancellationToken);


        var Specialleaves =
            await GetEmployeeSpecialLeavesAsync(
                filter,
                cancellationToken);



        var leaveLookup =
            BuildAttendanceLeaveLookup(leaves);

        var SpecialleaveLookup =
            BuildAttendanceSpecialLeaveLookup(Specialleaves);

            if (records == null)
            {
                return Result<byte[]>
                    .Failure("No Attendance Records Found");
            }

            var rows =
            records.Data?
            .Select(x =>
                CreateAttendanceRow(
                    x,
                    leaveLookup,
                    SpecialleaveLookup))
            .ToList();

            if (rows == null)
            {
                return Result<byte[]>
                    .Failure("No Attendance Records Found");
            }

            rows.Add(
            CreateTotalsRow(records.Data));



      
         var sheet = CreateSheet(
         "Employee Attendance",
          EmployeeAttendanceHeaders,
            rows);



        // =========================================================
        // Export
        // =========================================================

        var file =
            _excelExporter.Export(sheet);


        // =========================================================
        // Success
        // =========================================================

        return Result<byte[]>
            .Succeeded(file);
        }
        catch (Exception ex)
        {
            return Result<byte[]>
                .Failure(
                    $"Failed to export daily attendance: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>>
     ExportPerformanceAsync(
         int year,
         int month,
         CancellationToken cancellationToken = default)
    {
        try
        {
            // =========================================================
            // Validate Year
            // =========================================================

            if (year <= 0)
            {
                return Result<byte[]>.Failure(
                    "Year must be greater than zero.");
            }

            if (year < 2000 || year > 2100)
            {
                return Result<byte[]>.Failure(
                    "Year must be between 2000 and 2100.");
            }


            // =========================================================
            // Validate Month
            // =========================================================

            if (month < 0 || month > 12)
            {
                return Result<byte[]>.Failure(
                    "Month must be between 0 and 12.");
            }


            // =========================================================
            // Get Performance Records
            // =========================================================

            var records =
                await GetPerformanceRecordsAsync(
                    year,
                    month,
                    cancellationToken);


            // =========================================================
            // No Data
            // =========================================================

            if (records == null || records.Data.Count == 0)
            {
                var message =
                    month == 0
                        ? $"No performance data found for the year {year}."
                        : $"No performance data found for {year}-{month:D2}.";

                return Result<byte[]>.Failure(message);
            }


            // =========================================================
            // Create Rows
            // =========================================================

            var rows =
                records.Data?
                    .Select(CreatePerformanceRow)
                    .ToList();


            // =========================================================
            // Create Sheet
            // =========================================================

            var sheet =
                CreateSheet(
                    "Performance",
                    PerformanceHeaders,
                    rows);


            // =========================================================
            // Export
            // =========================================================

            var file =
                _excelExporter.Export(sheet);


            // =========================================================
            // Success
            // =========================================================

            return Result<byte[]>
                .Succeeded(file);
        }
        catch (Exception ex)
        {
            return Result<byte[]>
                .Failure(
                    $"Failed to export  attendance Performance: {ex.Message}");
        }
    }

    public async Task<Result<byte[]>> ExportMyAttendanceAsync(
    AttendanceFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        var employeeId = await _context.Employees
            .Where(x => x.UserId == _currentUserService.UserId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (employeeId == 0)
        {
            throw new InvalidOperationException("Employee not found.");
        }
        filter.EmployeeId = employeeId;

        return await ExportEmployeeAttendanceAsync(
            filter,
            cancellationToken);
    }

    public async Task<Result<byte[]>>
     ExportMyPerformanceAsync(
         int year,
         int month,
         CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Validate Year
        // =========================================================

        if (year <= 0)
        {
            return Result<byte[]>.Failure(
                "Year must be greater than zero.");
        }

        if (year < 2000 || year > 2100)
        {
            return Result<byte[]>.Failure(
                "Year must be between 2000 and 2100.");
        }


        // =========================================================
        // Validate Month
        // =========================================================

        if (month < 0 || month > 12)
        {
            return Result<byte[]>.Failure(
                "Month must be between 0 and 12. Use 0 to export the full year.");
        }


        // =========================================================
        // Get Current Employee
        // =========================================================

        var employeeId =
            await _context.Employees
                .AsNoTracking()
                .Where(x =>
                    x.UserId == _currentUserService.UserId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);


        // =========================================================
        // Employee Not Found
        // =========================================================

        if (employeeId <= 0)
        {
            return Result<byte[]>.Failure(
                "The current user is not associated with an employee.");
        }


        // =========================================================
        // Export Performance
        // =========================================================

        return await ExportEmployeePerformanceAsync(
            employeeId,
            year,
            month,
            cancellationToken);
    }

    public async Task<Result<byte[]>>
     ExportEmployeePerformanceAsync(
         int employeeId,
         int year,
         int month,
         CancellationToken cancellationToken = default)
    {
        // =========================================================
        // Validate Employee Id
        // =========================================================

        if (employeeId <= 0)
        {
            return Result<byte[]>.Failure(
                "Employee ID must be greater than zero.");
        }


        // =========================================================
        // Validate Year
        // =========================================================

        if (year <= 0)
        {
            return Result<byte[]>.Failure(
                "Year must be greater than zero.");
        }

        if (year < 2000 || year > 2100)
        {
            return Result<byte[]>.Failure(
                "Year must be between 2000 and 2100.");
        }


        // =========================================================
        // Validate Month
        // =========================================================

        // Month = 0 means all months of the year
        if (month < 0 || month > 12)
        {
            return Result<byte[]>.Failure(
                "Month must be between 0 and 12. Use 0 to export the full year.");
        }


        // =========================================================
        // Validate Employee
        // =========================================================

        var employeeExists =
            await _context.Employees
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == employeeId,
                    cancellationToken);

        if (!employeeExists)
        {
            return Result<byte[]>.Failure(
                "Employee not found.");
        }


        // =========================================================
        // Query Performance
        // =========================================================

        var query =
            _context.AttendancePerformances
                .AsNoTracking()
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Year == year);


        // =========================================================
        // Month Filter
        // =========================================================

        // If month = 0
        // do not filter by month
        if (month > 0)
        {
            query =
                query.Where(x =>
                    x.Month == month);
        }


        // =========================================================
        // Get Performance Records
        // =========================================================

        var records =
            await query
                .Select(x => new PerformanceExportDto
                {
                    AttendanceScore =
                        x.AttendanceScore,

                    PresentDays =
                        x.PresentDays,

                    AbsentDays =
                        x.AbsentDays,

                    EarlyLeaveDays =
                        x.EarlyLeaveDays,

                    AnnualLeaveDays =
                        x.AnnualLeaveDays,

                    SickLeaveDays =
                        x.SickLeaveDays,

                    UnpaidLeaveDays =
                        x.UnpaidLeaveDays,

                    PartialAttendanceDays =
                        x.PartialAttendanceDays,

                    MissingCheckInCount =
                        x.MissingCheckInCount,

                    MissingCheckOutCount =
                        x.MissingCheckOutCount,

                    TotalLateMinutes =
                         (int)x.TotalLateMinutes,
                        
                     TotalLostTimeMinutes =
                         (int)x.TotalLostTimeMinutes,
                        
                     TotalEarlyLeaveMinutes =
                         (int)x.TotalEarlyLeaveMinutes,
                        
                       TotalPenaltyPoints =
                         (int)x.TotalPenaltyPoints,


                    Notes =
                        x.Notes
                })
                .ToListAsync(cancellationToken);


        // =========================================================
        // No Data
        // =========================================================

        if (records.Count == 0)
        {
            var message =
                month > 0
                    ? $"No performance data found for employee {employeeId} for {year}-{month:D2}."
                    : $"No performance data found for employee {employeeId} for the year {year}.";

            return Result<byte[]>.Failure(message);
        }


        // =========================================================
        // Create Excel Rows
        // =========================================================

        var rows =
            records
                .Select(CreateMyPerformanceRow)
                .ToList();


        // =========================================================
        // Create Sheet
        // =========================================================

        var sheetName =
            month > 0
                ? $"Performance {month:D2}-{year}"
                : $"Performance {year}";

        var sheet =
            CreateSheet(
                sheetName,
                MyPerformanceHeaders,
                rows);


        // =========================================================
        // Export Excel
        // =========================================================

        var file =
            _excelExporter.Export(sheet);


        // =========================================================
        // Success
        // =========================================================

        return Result<byte[]>
            .Succeeded(file);
    }

    //=========================== Helpers ===============


    private static IQueryable<AttendanceRecord> ApplyAttendanceFilter(
    IQueryable<AttendanceRecord> query,
    AttendanceFilterDto filter)
    {
        if (filter.DateFrom.HasValue)
        {
            query = query.Where(x =>
                x.Date >= filter.DateFrom.Value);
        }


        if (filter.DateTo.HasValue)
        {
            query = query.Where(x =>
                x.Date <= filter.DateTo.Value);
        }


        if (filter.Status.HasValue)
        {
            query = query.Where(x =>
                x.Status == filter.Status.Value);
        }


        if (filter.EmployeeId.HasValue)
        {
            query = query.Where(x =>
                x.EmployeeId == filter.EmployeeId.Value);
        }


        if (filter.DepartmentId.HasValue)
        {
            query = query.Where(x =>
                x.Employee.DepartmentId ==
                filter.DepartmentId.Value);
        }


        return query;
    }

    private static ExcelSheetRow CreateMyPerformanceRow(
       PerformanceExportDto item)
    {
        return new ExcelSheetRow
        {
            Type = ExcelRowType.Data,

            Values =
            [
                item.AttendanceScore,

            item.PresentDays,

            item.AbsentDays,

            item.EarlyLeaveDays,

            item.AnnualLeaveDays,

            item.SickLeaveDays,

            item.UnpaidLeaveDays,

            item.PartialAttendanceDays,

            item.MissingCheckInCount,

            item.MissingCheckOutCount,

            item.TotalLateMinutes,

            item.TotalLostTimeMinutes,

            item.TotalEarlyLeaveMinutes,

            item.TotalPenaltyPoints,

            item.Notes ?? string.Empty
            ]
        };
    }

    private async Task<Result<List<PerformanceExportDto>>>
    GetPerformanceRecordsAsync(
    int year,
    int month,
    CancellationToken cancellationToken)
    {
        var result = await _context.AttendancePerformances
            .AsNoTracking()
            .Where(x =>
                x.Year == year &&
                x.Month == month)
            .OrderByDescending(x =>
                x.AttendanceScore)
            .Select(x => new PerformanceExportDto
            {
                Employee =
                    x.Employee.User.FullName,

                Department =
                    x.Employee.Department.NameAr,

                AttendanceScore =
                    x.AttendanceScore,

                PresentDays =
                    x.PresentDays,

                AbsentDays =
                    x.AbsentDays,

                PartialAttendanceDays =
                    x.PartialAttendanceDays,

                EarlyLeaveDays =
                    x.EarlyLeaveDays,

                AnnualLeaveDays =
                    x.AnnualLeaveDays,

                SickLeaveDays =
                    x.SickLeaveDays,

                UnpaidLeaveDays =
                    x.UnpaidLeaveDays,

                MissingCheckInCount =
                    x.MissingCheckInCount,

                MissingCheckOutCount =
                    x.MissingCheckOutCount,

                TotalLateMinutes =
                    x.TotalLateMinutes,

                TotalLostTimeMinutes =
                    x.TotalLostTimeMinutes,

                TotalEarlyLeaveMinutes =
                    x.TotalEarlyLeaveMinutes,

                TotalPenaltyPoints =
                    x.TotalPenaltyPoints,

                Notes =
                    x.Notes
            })
            .ToListAsync(cancellationToken);

        return Result<List<PerformanceExportDto>>
       .Succeeded(result);
    }

    private async Task<Result<List<AttendanceRecord>>> GetAttendanceRecordsAsync(
        AttendanceFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _context.AttendanceRecords
            .AsNoTracking()

            .Include(x => x.Employee)
                .ThenInclude(e => e.User)

            .Include(x => x.Transactions)

            .Where(x =>
                x.EmployeeId == filter.EmployeeId &&
                x.Date >= filter.DateFrom &&
                x.Date <= filter.DateTo)

            .OrderBy(x => x.Date)

            .ToListAsync(cancellationToken);
        return Result<List<AttendanceRecord>>
        .Succeeded(result);
    }


    private Task<List<EmployeeSpecialLeave>> GetEmployeeSpecialLeavesAsync(
    AttendanceFilterDto filter,
    CancellationToken cancellationToken)
    {
        return _context.EmployeeSpecialLeaves
            .AsNoTracking()
            .Where(x =>
                x.EmployeeId == filter.EmployeeId &&
                x.StartDate <= filter.DateTo &&
                x.EndDate >= filter.DateFrom)
            .ToListAsync(cancellationToken);
    }
    private Task<List<EmployeeLeave>> GetEmployeeLeavesAsync(
    AttendanceFilterDto filter,
    CancellationToken cancellationToken)
    {
        return _context.EmployeeLeaves
            .AsNoTracking()
            .Where(x =>
                x.EmployeeId == filter.EmployeeId &&
                x.Status == LeaveStatus.Approved &&
                x.StartDate <= filter.DateTo &&
                x.EndDate >= filter.DateFrom)
            .ToListAsync(cancellationToken);
    }
    private Task<List<EmployeeLeave>>
    GetAttendanceLeavesAsync(
    AttendanceFilterDto filter,
    CancellationToken cancellationToken)
    {
        var query =
            _context.EmployeeLeaves
            .AsNoTracking()
            .Where(x =>
                x.Status == LeaveStatus.Approved);

        if (filter.EmployeeId.HasValue)
        {
            query =
                query.Where(x =>
                    x.EmployeeId == filter.EmployeeId.Value);
        }

        if (filter.DateFrom.HasValue)
        {
            query =
                query.Where(x =>
                    x.EndDate >= filter.DateFrom.Value);
        }

        if (filter.DateTo.HasValue)
        {
            query =
                query.Where(x =>
                    x.StartDate <= filter.DateTo.Value);
        }

        return query.ToListAsync(cancellationToken);
    }

    private async Task<Result<List<EmployeeSpecialLeave>>>
    GetAttendanceSpecialLeavesAsync(
    AttendanceFilterDto filter,
    CancellationToken cancellationToken)
    {
        var query =
            _context.EmployeeSpecialLeaves
            .AsNoTracking();

        if (filter.EmployeeId.HasValue)
        {
            query =
                query.Where(x =>
                    x.EmployeeId == filter.EmployeeId.Value);
        }

        if (filter.DateFrom.HasValue)
        {
            query =
                query.Where(x =>
                    x.EndDate >= filter.DateFrom.Value);
        }

        if (filter.DateTo.HasValue)
        {
            query =
                query.Where(x =>
                    x.StartDate <= filter.DateTo.Value);
        }

        var result= await query.ToListAsync(cancellationToken);

        return Result<List<EmployeeSpecialLeave>>
          .Succeeded(result);
    }

    private static Dictionary<DateOnly, string> BuildAttendanceLeaveLookup(
     IEnumerable<EmployeeLeave> leaves)
    {
        var lookup =
            new Dictionary<DateOnly, string>();

        foreach (var leave in leaves)
        {
            for (var date = leave.StartDate;
                 date <= leave.EndDate;
                 date = date.AddDays(1))
            {
                lookup[date] =
                    leave.LeaveType.ToString();
            }
        }

        return lookup;
    }
    private static Dictionary<DateOnly, string> BuildAttendanceSpecialLeaveLookup(
        IEnumerable<EmployeeSpecialLeave> leaves)
    {
        var lookup =
            new Dictionary<DateOnly, string>();

        foreach (var leave in leaves)
        {
            for (var date = leave.StartDate;
                 date <= leave.EndDate;
                 date = date.AddDays(1))
            {
                lookup[date] =
                    leave.Type.ToString();
            }
        }

        return lookup;
    }

    private static string GetAttendanceStatus(
    AttendanceExportDto record,
    IReadOnlyDictionary<(int EmployeeId, DateOnly Date), string> leaveLookup,
    IReadOnlyDictionary<(int EmployeeId, DateOnly Date), string> specialLeaveLookup)
    {
        if (record.Status != AttendanceStatus.OnLeave)
        {
            return record.Status.ToString();
        }

        if (leaveLookup.TryGetValue(
            (record.EmployeeId, record.Date),
            out var leave))
        {
            return leave;
        }

        if (specialLeaveLookup.TryGetValue(
            (record.EmployeeId, record.Date),
            out var specialLeave))
        {
            return specialLeave;
        }

        return "On Leave";
    }
    private static Dictionary<(int EmployeeId, DateOnly Date), string> BuildLeaveLookup(
        IEnumerable<EmployeeLeave> leaves)
    {
        var lookup =
            new Dictionary<(int EmployeeId, DateOnly Date), string>();

        foreach (var leave in leaves)
        {
            for (var date = leave.StartDate;
                 date <= leave.EndDate;
                 date = date.AddDays(1))
            {
                lookup[(leave.EmployeeId, date)] =
                    leave.LeaveType.ToString();
            }
        }

        return lookup;
    }
    private static Dictionary<(int EmployeeId, DateOnly Date), string> BuildSpecialLeaveLookup(
        IEnumerable<EmployeeSpecialLeave> leaves)
    {
        var lookup =
            new Dictionary<(int EmployeeId, DateOnly Date), string>();

        foreach (var leave in leaves)
        {
            for (var date = leave.StartDate;
                 date <= leave.EndDate;
                 date = date.AddDays(1))
            {
                lookup[(leave.EmployeeId, date)] =
                    leave.Type.ToString();
            }
        }

        return lookup;
    }
    private static DateTime? GetCheckIn(
       IEnumerable<AttendanceTransaction>? transactions)
    {
        return transactions?
            .Where(x =>
                x.Type == AttendanceTransactionType.CheckIn)
            .OrderBy(x =>
                x.TransactionTime)
            .Select(x =>
                x.TransactionTime)
            .FirstOrDefault();
    }


    private static DateTime? GetCheckOut(
        IEnumerable<AttendanceTransaction>? transactions)
    {
        return transactions?
            .Where(x =>
                x.Type == AttendanceTransactionType.CheckOut)
            .OrderByDescending(x =>
                x.TransactionTime)
            .Select(x =>
                x.TransactionTime)
            .FirstOrDefault();
    }

    private static string GetStatus(
        AttendanceStatus status,
        DateOnly date,
        IReadOnlyDictionary<DateOnly, string> leaveLookup,
        IReadOnlyDictionary<DateOnly, string> specialLeaveLookup)
    {
        if (status != AttendanceStatus.OnLeave)
        {
            return status.ToString();
        }

        if (leaveLookup.TryGetValue(date, out var leaveType))
        {
            return leaveType;
        }

        if (specialLeaveLookup.TryGetValue(date, out var specialLeaveType))
        {
            return specialLeaveType;
        }

        return "On Leave";
    }


    private static ExcelSheetRow CreateTotalsRow(
       IEnumerable<AttendanceRecord> records)
    {
        var workingRecords =
            records.Where(x =>
                x.Status != AttendanceStatus.Weekend &&
                x.Status != AttendanceStatus.Holiday);

        var expectedMinutes =
            workingRecords.Sum(x =>
                x.ExpectedMinutes);

        var workedMinutes =
            workingRecords.Sum(x =>
                x.WorkedMinutes);

        var lateMinutes =
            workingRecords.Sum(x =>
                x.LateMinutes);

        var lostTimeMinutes =
            Math.Max(
                expectedMinutes - workedMinutes,
                0);

        var earlyLeaveMinutes =
            workingRecords.Sum(x =>
                x.EarlyLeaveMinutes);

        return new ExcelSheetRow
        {
            Type = ExcelRowType.Total,

            Values =
            [
                "TOTAL",
            "",
            "",
            "",

            expectedMinutes,
            workedMinutes,
            lateMinutes,
            lostTimeMinutes,
            earlyLeaveMinutes
            ]
        };
    }

    private static (DateOnly From, DateOnly To) GetDateRange(
    AttendanceFilterDto filter)
    {
        return
        (
            filter.DateFrom ??
            new DateOnly(
                DateTime.Today.Year,
                DateTime.Today.Month,
                1),

            filter.DateTo ??
            DateOnly.FromDateTime(
                DateTime.Today)
        );
    }

    private static ExcelSheetData CreateSheet(
    string name,
    IReadOnlyList<string> headers,
    IReadOnlyList<ExcelSheetRow> rows,
    IReadOnlyList<IReadOnlyList<object?>>? topRows = null,
    string? sectionTitle = null)
    {
        return new ExcelSheetData
        {
            SheetName = name,

            Headers =
                headers.ToList(),

            Rows =
                rows.ToList(),

            TopRows =
                topRows?.ToList() ?? [],

            SectionTitle =
                sectionTitle
        };
    }

    private async Task<Result<List<DailyAttendanceExportDto>>>
    GetDailyAttendanceRecordsAsync(
    DateOnly date,
    CancellationToken cancellationToken)
    {
        var result= await _context.AttendanceRecords
            .AsNoTracking()
            .Where(x =>
                x.Date == date)
            .OrderBy(x =>
                x.Employee.User.FullName)
            .Select(x => new DailyAttendanceExportDto
            {
                Employee =
                    x.Employee.User.FullName,

                Department =
                    x.Employee.Department.NameAr,

     

                CheckIn =
                    x.Transactions
                    .Where(t =>
                        t.Type == AttendanceTransactionType.CheckIn)
                    .OrderBy(t =>
                        t.TransactionTime)
                    .Select(t =>
                        t.TransactionTime)
                    .FirstOrDefault(),

                CheckOut =
                    x.Transactions
                    .Where(t =>
                        t.Type == AttendanceTransactionType.CheckOut)
                    .OrderByDescending(t =>
                        t.TransactionTime)
                    .Select(t =>
                        t.TransactionTime)
                    .FirstOrDefault(),

                WorkedMinutes =
                    x.WorkedMinutes,

                LateMinutes =
                    x.LateMinutes,

                EarlyLeaveMinutes =
                    x.EarlyLeaveMinutes,

                Status =
                    x.Status
            })
            .ToListAsync(cancellationToken);
        return Result<List<DailyAttendanceExportDto>>
           .Succeeded(result);
    }

    private static string FormatTime(
    DateTime? time)
    {
        return time.HasValue
            ? time.Value.ToString("HH:mm")
            : string.Empty;
    }

    private static IReadOnlyList<string> EmployeeAttendanceHeaders =>
[
    "Date",
    "Status",
    "Check In",
    "Check Out",
    "Expected Minutes",
    "Worked Minutes",
    "Late Minutes",
    "Lost Time",
    "Early Leave"
];
    private static IReadOnlyList<string> DailyAttendanceHeaders =>
   [
       "Employee",
    "Department",
     "Status",
    "Check In",
    "Check Out",
    "Worked Minutes",
    "Late Minutes",
    "Early Leave",
    
   ];
    private static IReadOnlyList<string> AttendanceHeaders =>
     [
         "Employee",
    "Department",
    "Date",
    "Status",
    "Check In",
    "Check Out",
    "Expected Minutes",
    "Worked Minutes",
    "Late Minutes",
    "Lost Time",
    "Early Leave",
    "Notes"
     ];
    private static IReadOnlyList<string> PerformanceHeaders =>
[
    "Employee",
    "Department",
    "Final Score",
    "Present Days",
    "Absent Days",
    "Partial Attendance",
    "Early Leave Days",
    "Annual Leave Days",
    "Sick Leave Days",
    "Unpaid Leave Days",
    "Missing Check In",
    "Missing Check Out",
    "Late Minutes",
    "Lost Time Minutes",
    "Early Leave Minutes",
    "Penalty Points",
    "Notes"
];
    private static readonly IReadOnlyList<string> MyPerformanceHeaders =
[
    "Final Score",
    "Present Days",
    "Absent Days",
    "Early Leave Days",
    "Annual Leave Days",
    "Sick Leave Days",
    "Unpaid Leave Days",
    "Partial Attendance Days",
    "Missing Check In",
    "Missing Check Out",
    "Late Minutes",
    "Lost Time Minutes",
    "Early Leave Minutes",
    "Penalty Points",
    "Notes"
];

    private sealed class DailyAttendanceExportDto
    {
        public int EmployeeId { get; set; }

        public string Employee { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public int WorkedMinutes { get; set; }

        public int LateMinutes { get; set; }

        public int EarlyLeaveMinutes { get; set; }

        public AttendanceStatus Status { get; set; }
    }

    private sealed class AttendanceExportDto
    {
        public int EmployeeId { get; set; }

        public string Employee { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public DateOnly Date { get; set; }

        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public int ExpectedMinutes { get; set; }

        public int WorkedMinutes { get; set; }

        public int LateMinutes { get; set; }

        public int LostTimeMinutes { get; set; }

        public int EarlyLeaveMinutes { get; set; }

        public AttendanceStatus Status { get; set; }

        public string? Notes { get; set; }
    }

   

    private sealed class PerformanceExportDto
    {
        public string Employee { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public int AttendanceScore { get; set; }

        public int PresentDays { get; set; }

        public int AbsentDays { get; set; }

        public int PartialAttendanceDays { get; set; }

        public int EarlyLeaveDays { get; set; }

        public int AnnualLeaveDays { get; set; }

        public int SickLeaveDays { get; set; }

        public int UnpaidLeaveDays { get; set; }

        public int MissingCheckInCount { get; set; }

        public int MissingCheckOutCount { get; set; }

        public int TotalLateMinutes { get; set; }

        public int TotalLostTimeMinutes { get; set; }

        public int TotalEarlyLeaveMinutes { get; set; }

        public decimal TotalPenaltyPoints { get; set; }

        public string? Notes { get; set; }
    }

    private static ExcelSheetRow CreateAttendanceExportRow(
        AttendanceExportDto record,
        IReadOnlyDictionary<(int EmployeeId, DateOnly Date), string> leaveLookup,
        IReadOnlyDictionary<(int EmployeeId, DateOnly Date), string> specialLeaveLookup)
    {
        return new ExcelSheetRow
        {
            Type = ExcelRowType.Data,

            Values =
            [
                record.Employee,

            record.Department,

            record.Date,

            GetAttendanceStatus(
                record,
                leaveLookup,
                specialLeaveLookup),

            FormatTime(record.CheckIn),

            FormatTime(record.CheckOut),

            record.ExpectedMinutes,

            record.WorkedMinutes,

            record.LateMinutes,

            record.LostTimeMinutes,

            record.EarlyLeaveMinutes,

            record.Notes ?? string.Empty
            ]
        };
    }
    private static ExcelSheetRow CreateAttendanceRow(
     AttendanceRecord record,
     IReadOnlyDictionary<DateOnly, string> leaveLookup,
     IReadOnlyDictionary<DateOnly, string> specialLeaveLookup)
    {
        return new ExcelSheetRow
        {
            Type = ExcelRowType.Data,

            Values =
            [
                record.Date,

            GetStatus(
                record.Status,
                record.Date,
                leaveLookup,
                specialLeaveLookup),

            FormatTime(
                GetCheckIn(record.Transactions)),

            FormatTime(
                GetCheckOut(record.Transactions)),

            record.ExpectedMinutes,

            record.WorkedMinutes,

            record.LateMinutes,

            record.LostTimeMinutes,

            record.EarlyLeaveMinutes
            ]
        };
    }



    private static ExcelSheetRow CreateDailyAttendanceRow(
        DailyAttendanceExportDto item)
    {
        return new ExcelSheetRow
        {
            Type = ExcelRowType.Data,

            Values =
            [
                item.Employee,

            item.Department,

            item.Status.ToString(),

            FormatTime(item.CheckIn),

            FormatTime(item.CheckOut),

            item.WorkedMinutes,

            item.LateMinutes,

            item.EarlyLeaveMinutes

         
            ]
        };
    }


    private static ExcelSheetRow CreatePerformanceRow(
     PerformanceExportDto item)
    {
        return new ExcelSheetRow
        {
            Type = ExcelRowType.Data,

            Values =
            [
                item.Employee,

            item.Department,

            item.AttendanceScore,

            item.PresentDays,

            item.AbsentDays,

            item.PartialAttendanceDays,

            item.EarlyLeaveDays,

            item.AnnualLeaveDays,

            item.SickLeaveDays,

            item.UnpaidLeaveDays,

            item.MissingCheckInCount,

            item.MissingCheckOutCount,

            item.TotalLateMinutes,

            item.TotalLostTimeMinutes,

            item.TotalEarlyLeaveMinutes,

            item.TotalPenaltyPoints,

            item.Notes ?? string.Empty
            ]
        };
    }


}