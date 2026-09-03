using Microsoft.EntityFrameworkCore;
using MicroERP.Application.Common.Models;

namespace MicroERP.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
      this IQueryable<T> query,
      PagedRequest request,
      CancellationToken cancellationToken = default)
    {
        request.PageNumber =
            Math.Max(1, request.PageNumber);

        request.PageSize =
            Math.Clamp(request.PageSize, 1, 200);

        var totalCount =
            await query.CountAsync(cancellationToken);

        var items =
            await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize);
    }
}