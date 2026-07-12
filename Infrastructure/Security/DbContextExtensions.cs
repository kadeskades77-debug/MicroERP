using MicroERP.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Infrastructure.Security
{
    public static class DbContextExtensions
    {
        public static async Task<TResult> ExecuteSafeAsync<TResult>(
            this IApplicationDbContext context,
            Func<Task<TResult>> action)
        {
            var strategy = context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(action);
        }
    }
}
