using System.Linq.Expressions;

namespace MicroERP.Application.Common.Interfaces;

public interface IGlobalQueryService
{
    Task<bool> IsExistsAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : class;
}