using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Application.Features.Ticketing.DTOs;
using MicroERP.Application.Features.Ticketing.Interfaces;
using MicroERP.Application.Features.Ticketing.Mappings;
using MicroERP.Domain.Audit;
using MicroERP.Domin.Entities.Ticketing;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;


namespace MicroERP.Application.Features.Ticketing.Services;

public class TicketAssignmentService : ITicketAssignmentService
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public TicketAssignmentService(
        IApplicationDbContext context,
        IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    // =========================================================
    // Assign Ticket
    // =========================================================

    public async Task<Result<TicketDto>> AssignAsync(
     AssignTicketDto dto,
     CancellationToken ct = default)
    {
        // =========================================================
        // Validate Request
        // =========================================================

        if (dto is null)
        {
            return Result<TicketDto>.Failure(
                "Request is required.");
        }

        if (dto.TicketId <= 0)
        {
            return Result<TicketDto>.Failure(
                "Invalid ticket ID.");
        }

        if (dto.EmployeeId <= 0)
        {
            return Result<TicketDto>.Failure(
                "Invalid employee ID.");
        }

        // =========================================================
        // Get Ticket
        // =========================================================

        var ticket =
            await _context.Tickets
                .FirstOrDefaultAsync(
                    x => x.Id == dto.TicketId,
                    ct);

        if (ticket is null)
        {
            return Result<TicketDto>.Failure(
                "Ticket not found.");
        }

        // =========================================================
        // Validate Ticket Status
        // =========================================================

        if (ticket.Status == TicketStatus.Closed)
        {
            return Result<TicketDto>.Failure(
                "Closed tickets cannot be assigned.");
        }

        if (ticket.Status == TicketStatus.Cancelled)
        {
            return Result<TicketDto>.Failure(
                "Cancelled tickets cannot be assigned.");
        }

        // =========================================================
        // Get Employee
        // =========================================================

        var employee =
            await _context.Employees
                .AsNoTracking()
                .Include(x => x.User)
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == dto.EmployeeId &&
                        x.IsActive,
                    ct);

        if (employee is null)
        {
            return Result<TicketDto>.Failure(
                "Employee not found or inactive.");
        }

        // =========================================================
        // Get Current Assignment
        // =========================================================

        var currentAssignment =
            await _context.TicketAssignments
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(
                    x =>
                        x.TicketId == ticket.Id &&
                        x.IsCurrent,
                    ct);

        // =========================================================
        // Prevent Duplicate Assignment
        // =========================================================

        if (currentAssignment is not null &&
            currentAssignment.EmployeeId == dto.EmployeeId)
        {
            return Result<TicketDto>.Failure(
                "Ticket is already assigned to this employee.");
        }

        // =========================================================
        // Store Old Values
        // =========================================================

        var oldValues = new
        {
            AssignedToEmployeeId =
                ticket.AssignedToEmployeeId,

            AssignedDepartmentId =
                ticket.AssignedDepartmentId,

            Status =
                ticket.Status.ToString()
        };

        var oldEmployeeName =
            currentAssignment?
                .Employee?
                .User?
                .FullName;

        var newEmployeeName =
            employee.User?.FullName;

        // =========================================================
        // Close Previous Assignment
        // =========================================================

        if (currentAssignment is not null)
        {
            currentAssignment.IsCurrent = false;
            currentAssignment.UnassignedOn =
                DateTime.UtcNow;
        }

        // =========================================================
        // Create New Assignment
        // =========================================================

        var assignment =
            new TicketAssignment
            {
                TicketId = ticket.Id,

                EmployeeId = employee.Id,

                DepartmentId =
                    employee.DepartmentId,

                IsCurrent = true
            };

        await _context.TicketAssignments.AddAsync(
            assignment,
            ct);

        // =========================================================
        // Update Ticket
        // =========================================================

        ticket.AssignedToEmployeeId =
            employee.Id;

        ticket.AssignedDepartmentId =
            employee.DepartmentId;

        ticket.Status =
            TicketStatus.Assigned;

        // =========================================================
        // Create History
        // =========================================================

        await CreateHistoryAsync(
            ticket.Id,
            TicketHistoryAction.Assigned,
            oldEmployeeName,
            newEmployeeName,
            dto.Notes,
            ct);

        // =========================================================
        // Save Changes
        // =========================================================

        await _context.SaveChangesAsync(ct);

        // =========================================================
        // Audit
        // =========================================================

        await _auditService.LogAsync(
            AuditActions.Update,
            nameof(Ticket),
            ticket.Id.ToString(),
            oldValues,
            new
            {
                AssignedToEmployeeId =
                    ticket.AssignedToEmployeeId,

                AssignedDepartmentId =
                    ticket.AssignedDepartmentId,

                Status =
                    ticket.Status.ToString(),

                AssignmentId =
                    assignment.Id,

                EmployeeId =
                    employee.Id
            });

        // =========================================================
        // Get Updated Ticket
        // =========================================================

        var result =
            await _context.Tickets
                .AsNoTracking()
                .Where(x => x.Id == ticket.Id)
                .Select(x => new TicketDto
                {
                    Id = x.Id,

                    TicketNumber =
                        x.TicketNumber,

                    Title =
                        x.Title,

                    Description =
                        x.Description,

                    Status =
                        x.Status,

                    Priority =
                        x.Priority,

                    CategoryId =
                        x.CategoryId,

                    CategoryName =
                        x.Category != null
                            ? x.Category.Name
                            : string.Empty,

                    AssignedToEmployeeId =
                        x.AssignedToEmployeeId,

                    AssignedToEmployeeName =
                        x.AssignedToEmployee != null
                            ? x.AssignedToEmployee.User.FullName
                            : null,

                    AssignedDepartmentId =
                        x.AssignedDepartmentId,

                    AssignedDepartmentName =
                        x.AssignedDepartment != null
                            ? x.AssignedDepartment.NameEn
                            : null,

                    DueOn =
                        x.DueOn,

                    ResolvedOn =
                        x.ResolvedOn,

                    ClosedOn =
                        x.ClosedOn,

                    CreatedOn =
                        x.CreatedOn
                })
                .FirstAsync(ct);

        // =========================================================
        // Return
        // =========================================================

        return Result<TicketDto>.Succeeded(
            result,
            "Ticket assigned successfully.");
    }

    // =========================================================
    // Unassign Ticket
    // =========================================================

    public async Task<Result<TicketDto>> UnassignAsync(
       UnassignTicketDto dto,
       CancellationToken ct = default)
    {
        // =====================================================
        // Validate Request
        // =====================================================

        if (dto is null)
        {
            return Result<TicketDto>.Failure(
                "Request is required.");
        }

        if (dto.TicketId <= 0)
        {
            return Result<TicketDto>.Failure(
                "Invalid ticket ID.");
        }

        // =====================================================
        // Get Ticket
        // =====================================================

        var ticket =
            await _context.Tickets
                .FirstOrDefaultAsync(
                    x => x.Id == dto.TicketId,
                    ct);

        if (ticket is null)
        {
            return Result<TicketDto>.Failure(
                "Ticket not found.");
        }

        // =====================================================
        // Validate Ticket Status
        // =====================================================

        if (ticket.Status != TicketStatus.Assigned)
        {
            return Result<TicketDto>.Failure(
                "Only assigned tickets can be unassigned.");
        }
    

        if (ticket.Status == TicketStatus.Closed)
        {
            return Result<TicketDto>.Failure(
                "Closed tickets cannot be unassigned.");
        }
        // =====================================================
        // Get Current Assignment
        // =====================================================

        var assignment =
            await _context.TicketAssignments
                .Include(x => x.Employee)
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(
                    x =>
                        x.TicketId == ticket.Id &&
                        x.IsCurrent,
                    ct);

        if (assignment is null)
        {
            return Result<TicketDto>.Failure(
                "Ticket has no current assignment.");
        }

        // =====================================================
        // Old Values
        // =====================================================

        var oldEmployeeId =
            ticket.AssignedToEmployeeId;

        var oldDepartmentId =
            ticket.AssignedDepartmentId;

        var oldEmployeeName =
            assignment.Employee?.User?.FullName;

        // =====================================================
        // Unassign
        // =====================================================

        assignment.IsCurrent = false;
        assignment.UnassignedOn = DateTime.UtcNow;

        ticket.AssignedToEmployeeId = null;
        ticket.AssignedDepartmentId = null;

        ticket.Status = TicketStatus.Open;

        // =====================================================
        // Create History
        // =====================================================

        await CreateHistoryAsync(
          ticket.Id,
          TicketHistoryAction.Unassigned,
          oldEmployeeName,
          null,
          dto.Notes,
          ct);

        // =====================================================
        // Save
        // =====================================================

        await _context.SaveChangesAsync(ct);

        // =====================================================
        // Audit
        // =====================================================

        await _auditService.LogAsync(
            AuditActions.Update,
            nameof(Ticket),
            ticket.Id.ToString(),
            new
            {
                AssignedToEmployeeId = oldEmployeeId,
                AssignedDepartmentId = oldDepartmentId,
                Status = TicketStatus.Assigned.ToString()
            },
            new
            {
                AssignedToEmployeeId = (int?)null,
                AssignedDepartmentId = (int?)null,
                Status = TicketStatus.Open.ToString(),
                Notes = dto.Notes
            });

       

        // =====================================================
        // Result
        // =====================================================

        return Result<TicketDto>.Succeeded(
            ticket.ToDto(),
            "Ticket unassigned successfully.");
    }

    // =========================================================
    // Get Assignment History
    // =========================================================

    public async Task<Result<List<TicketAssignmentDto>>>
        GetHistoryAsync(
            int ticketId,
            CancellationToken ct = default)
    {
        // =====================================================
        // Validate
        // =====================================================

        if (ticketId <= 0)
        {
            return Result<List<TicketAssignmentDto>>.Failure(
                "Invalid ticket ID.");
        }

        // =====================================================
        // Check Ticket
        // =====================================================

        var ticketExists =
            await _context.Tickets
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == ticketId,
                    ct);

        if (!ticketExists)
        {
            return Result<List<TicketAssignmentDto>>.Failure(
                "Ticket not found.");
        }

        // =====================================================
        // Get History
        // =====================================================

        var history =
            await _context.TicketAssignments
                .AsNoTracking()
                .Where(
                    x =>
                        x.TicketId == ticketId)
                .OrderByDescending(
                    x => x.CreatedOn)
                .Select(
                    x => new TicketAssignmentDto
                    {
                        Id =
                            x.Id,

                        TicketId =
                            x.TicketId,

                        EmployeeId =
                            x.EmployeeId,

                        EmployeeName =
                            x.Employee.User.FullName,

                        DepartmentId =
                            x.DepartmentId,

                        DepartmentName =
                            x.Department != null
                                ? x.Department.NameEn
                                : null,

                        IsCurrent =
                            x.IsCurrent,

                        CreatedOn =
                            x.CreatedOn,

                        UnassignedOn =
                            x.UnassignedOn
                    })
                .ToListAsync(ct);

        // =====================================================
        // Result
        // =====================================================

        return Result<List<TicketAssignmentDto>>.Succeeded(
            history);
    }

    private async Task CreateHistoryAsync(
    int ticketId,
    TicketHistoryAction action,
    string? oldValue = null,
    string? newValue = null,
    string? notes = null,
    CancellationToken ct = default)
    {
        // =====================================================
        // Create History
        // =====================================================

        var history = new TicketHistory
        {
            TicketId = ticketId,
            Action = action,
            OldValue = oldValue,
            NewValue = newValue,
            Notes = notes
        };

        // =====================================================
        // Add History
        // =====================================================

        await _context.TicketHistories.AddAsync(
            history,
            ct);
    }



}

