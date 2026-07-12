using System.Text.Json;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Audit.DTOs;
using MicroERP.Application.Features.Audit.Interfaces;
using MicroERP.Domain.Audit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserService _currentUser;

    public AuditService(
        IApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserService currentUser)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _currentUser = currentUser;
    }


    //================ GET ALL =================

    public async Task<Result<PagedResult<AuditLogDto>>> GetAllAsync(
        AuditLogFilterDto dto)
    {
        var query = _context.AuditLogs
            .AsNoTracking()
            .AsQueryable();


        if (!string.IsNullOrWhiteSpace(dto.UserId))
        {
            query = query.Where(x =>
                x.UserId == dto.UserId);
        }


        if (!string.IsNullOrWhiteSpace(dto.Action))
        {
            query = query.Where(x =>
                x.Action == dto.Action);
        }


        if (!string.IsNullOrWhiteSpace(dto.EntityName))
        {
            query = query.Where(x =>
                x.EntityName == dto.EntityName);
        }


        if (dto.FromDate.HasValue)
        {
            query = query.Where(x =>
                x.CreatedOn >= dto.FromDate.Value);
        }


        if (dto.ToDate.HasValue)
        {
            query = query.Where(x =>
                x.CreatedOn <= dto.ToDate.Value);
        }


        var totalCount = await query.CountAsync();


        var data = await query
            .OrderByDescending(x => x.CreatedOn)
            .Skip((dto.PageNumber - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .Select(x => new AuditLogDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.UserName,
                Action = x.Action,
                EntityName = x.EntityName,
                EntityId = x.EntityId,
                CreatedOn = x.CreatedOn
            })
            .ToListAsync();


        return Result<PagedResult<AuditLogDto>>
            .Succeeded(new PagedResult<AuditLogDto>
            {
                Items = data,
                TotalCount = totalCount,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize
            });
    }


    //================ GET BY ID =================

    public async Task<Result<AuditLogDetailsDto>> GetByIdAsync(
        int id)
    {
        var audit = await _context.AuditLogs
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AuditLogDetailsDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.UserName,
                Action = x.Action,
                EntityName = x.EntityName,
                EntityId = x.EntityId,
                OldValues = x.OldValues,
                NewValues = x.NewValues,
                IpAddress = x.IpAddress,
                UserAgent = x.UserAgent,
                CreatedOn = x.CreatedOn
            })
            .FirstOrDefaultAsync();


        if (audit is null)
            return Result<AuditLogDetailsDto>
                .Failure("Audit log not found.");


        return Result<AuditLogDetailsDto>
            .Succeeded(audit);
    }


    //================ LOG =================

    public Task LogAsync(
     string action,
     string entityName,
     string? entityId = null,
     object? oldValues = null,
     object? newValues = null)
    {
        var httpContext =
            _httpContextAccessor.HttpContext;


        var audit = new AuditLog
        {
            UserId = _currentUser.UserId,

            UserName = _currentUser.UserName,

            Action = action,

            EntityName = entityName,

            EntityId = entityId,

            OldValues = oldValues is null
                ? null
                : JsonSerializer.Serialize(oldValues),

            NewValues = newValues is null
                ? null
                : JsonSerializer.Serialize(newValues),

            IpAddress =
                httpContext?
                    .Connection
                    .RemoteIpAddress?
                    .ToString(),

            UserAgent =
                httpContext?
                    .Request
                    .Headers["User-Agent"]
                    .ToString()
        };


        _context.AuditLogs.Add(audit);

        return Task.CompletedTask;
    }
}