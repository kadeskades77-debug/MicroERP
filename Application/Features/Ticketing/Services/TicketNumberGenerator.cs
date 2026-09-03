using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Ticketing.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MicroERP.Application.Features.Ticketing.Services;

public class TicketNumberGenerator : ITicketNumberGenerator
{
    private readonly IApplicationDbContext _context;

    public TicketNumberGenerator(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateAsync(
        CancellationToken ct = default)
    {
        var year =
            DateTime.UtcNow.Year;

        var sequenceValue =
            await GetNextSequenceValueAsync(ct);

        return $"TKT-{year}-{sequenceValue:D6}";
    }

    private async Task<int> GetNextSequenceValueAsync(
        CancellationToken ct)
    {
        var connection =
            _context.Database.GetDbConnection();

        await using var command =
            connection.CreateCommand();

        command.CommandText =
            "SELECT NEXT VALUE FOR dbo.TicketNumberSequence";

        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(ct);
        }

        var result =
            await command.ExecuteScalarAsync(ct);

        return Convert.ToInt32(result);
    }
}

