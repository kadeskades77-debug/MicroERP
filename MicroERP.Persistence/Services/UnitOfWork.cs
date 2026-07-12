using MicroERP.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Persistence.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ExecuteAsync(Func<Task> action)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            await action();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var result = await action();

            await transaction.CommitAsync();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}