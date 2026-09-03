using AutoMapper;
using AutoMapper.QueryableExtensions;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.DTOs;
using MicroERP.Application.Features.EmployeeAttendance.Attendance.Interfaces;
using MicroERP.Application.Features.EmployeeAttendance.AttendanceLogs.Calculators;
using MicroERP.Domin.Entities.EmployeeAttendance;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.EmployeeAttendance.Attendance.Services;

public class AttendanceCorrectionService
    : IAttendanceCorrectionService
{
    private readonly IApplicationDbContext _context;
    private readonly IAttendanceCalculator _calculator;
    private readonly IMapper _mapper;
    private readonly IAuditService _auditService;


    public AttendanceCorrectionService(
        IApplicationDbContext context,
        IAttendanceCalculator calculator,
        IMapper mapper,
        IAuditService auditService)
    {
        _context = context;
        _calculator = calculator;
        _mapper = mapper;
        _auditService = auditService;
    }



    public async Task<Result<AttendanceCorrectionDto>> CreateAsync(
        CreateAttendanceCorrectionDto dto,
        CancellationToken cancellationToken = default)
    {
        var attendance =
            await _context.AttendanceRecords
            .Include(x => x.Employee)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(
                x => x.Id == dto.AttendanceRecordId,
                cancellationToken);



        if (attendance == null)
        {
            return Result<AttendanceCorrectionDto>
                .Failure("Attendance record not found");
        }



        var correction = new AttendanceCorrection
        {
            AttendanceRecordId = attendance.Id,


            OldCheckIn = null,
            OldCheckOut = null,
            OldStatus = attendance.Status,


            NewCheckIn = dto.NewCheckIn,
            NewCheckOut = dto.NewCheckOut,
            NewStatus = dto.NewStatus,

            ShiftNumber = dto.ShiftNumber,

            Reason = dto.Reason,


            Status = AttendanceCorrectionStatus.Pending,


            // مؤقتًا من الـ Claims لاحقًا
            RequestedByUserId = ""
        };



        await _context.AttendanceCorrections
            .AddAsync(
                correction,
                cancellationToken);



        await _context.SaveChangesAsync(
            cancellationToken);



        return Result<AttendanceCorrectionDto>
    .Succeeded(_mapper.Map<AttendanceCorrectionDto>(correction));
    }




    public async Task<Result<bool>> ApproveAsync(
     int correctionId,
     CancellationToken cancellationToken = default)
    {
        var correction = await _context.AttendanceCorrections
            .Include(x => x.AttendanceRecord)
            .FirstOrDefaultAsync(
                x => x.Id == correctionId,
                cancellationToken);

        if (correction == null)
            return Result<bool>.Failure("Correction not found");

        if (correction.Status != AttendanceCorrectionStatus.Pending)
            return Result<bool>.Failure("Correction already processed");

        var attendance = correction.AttendanceRecord;

        var transactions = await _context.AttendanceTransactions
            .Where(x => x.AttendanceRecordId == attendance.Id)
            .ToListAsync(cancellationToken);

        var oldCheckIn =
    transactions
    .FirstOrDefault(x =>
        x.Type == AttendanceTransactionType.CheckIn)
    ?.TransactionTime;


        var oldCheckOut =
            transactions
            .LastOrDefault(x =>
                x.Type == AttendanceTransactionType.CheckOut)
            ?.TransactionTime;

        // =========================
        // CheckIn
        // =========================
        if (correction.NewCheckIn.HasValue)
        {
            var checkIn = transactions.FirstOrDefault(x =>
                x.Type == AttendanceTransactionType.CheckIn);

            if (checkIn == null)
            {
                checkIn = new AttendanceTransaction
                {
                    AttendanceRecordId = attendance.Id,
                    ShiftNumber = correction.ShiftNumber,
                    Type = AttendanceTransactionType.CheckIn,
                    TransactionTime = attendance.Date.ToDateTime(
                        correction.NewCheckIn.Value)
                };

                await _context.AttendanceTransactions.AddAsync(
                    checkIn,
                    cancellationToken);
            }
            else
            {
                checkIn.TransactionTime =
                    attendance.Date.ToDateTime(
                        correction.NewCheckIn.Value);
            }
        }

        // =========================
        // CheckOut
        // =========================
        if (correction.NewCheckOut.HasValue)
        {
            var checkOut = transactions.LastOrDefault(x =>
                x.Type == AttendanceTransactionType.CheckOut);

            if (checkOut == null)
            {
                checkOut = new AttendanceTransaction
                {
                    AttendanceRecordId = attendance.Id,
                    ShiftNumber = correction.ShiftNumber,
                    Type = AttendanceTransactionType.CheckOut,
                    TransactionTime = attendance.Date.ToDateTime(
                        correction.NewCheckOut.Value)
                };

                await _context.AttendanceTransactions.AddAsync(
                    checkOut,
                    cancellationToken);
            }
            else
            {
                checkOut.TransactionTime =
                    attendance.Date.ToDateTime(
                        correction.NewCheckOut.Value);
            }
        }

        // =========================
        // Status
        // =========================
        if (correction.NewStatus.HasValue)
        {
            attendance.Status = correction.NewStatus.Value;
        }

        correction.Status = AttendanceCorrectionStatus.Approved;
        correction.ApprovedOn = DateTime.UtcNow;

        await _calculator.CalculateAsync(
            attendance,
            cancellationToken);
        await _auditService.LogAsync(
    "AttendanceCorrection.Approve",
    $"""
    Attendance correction approved.

    RecordId: {attendance.Id}

    Old CheckIn:
    {oldCheckIn}

    New CheckIn:
    {correction.NewCheckIn}

    Old CheckOut:
    {oldCheckOut}

    New CheckOut:
    {correction.NewCheckOut}

    Reason:
    {correction.Reason}
    """);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Succeeded(true);
    }





    public async Task<Result<bool>> RejectAsync(
        int correctionId,
        string rejectionReason,
        CancellationToken cancellationToken = default)
    {
        var correction =
            await _context.AttendanceCorrections
            .FirstOrDefaultAsync(
                x => x.Id == correctionId,
                cancellationToken);



        if (correction == null)
        {
            return Result<bool>
                .Failure("Correction not found");
        }



        correction.Status =
            AttendanceCorrectionStatus.Rejected;



        correction.RejectionReason =
            rejectionReason;

        await _auditService.LogAsync(
    "AttendanceCorrection.Reject",
    $"""
    Attendance correction rejected.

    CorrectionId:
    {correction.Id}

    Reason:
    {rejectionReason}
    """
   );

        await _context.SaveChangesAsync(
            cancellationToken);



        return Result<bool>
            .Succeeded(true);
    }





    public async Task<Result<List<AttendanceCorrectionDto>>> GetPendingAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _context.AttendanceCorrections
    .Include(x => x.AttendanceRecord)
        .ThenInclude(x => x.Employee)
            .ThenInclude(x => x.User)
    .Where(x => x.Status == AttendanceCorrectionStatus.Pending)
    .ProjectTo<AttendanceCorrectionDto>(_mapper.ConfigurationProvider)
    .ToListAsync(cancellationToken);



        if (result == null)
        {
            return Result<List<AttendanceCorrectionDto>>
                .Failure("Correction not found");
        }

        return Result<List<AttendanceCorrectionDto>>
            .Succeeded(result);
    }
}