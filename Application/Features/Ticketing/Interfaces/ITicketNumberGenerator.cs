namespace MicroERP.Application.Features.Ticketing.Interfaces;

public interface ITicketNumberGenerator
{
    Task<string> GenerateAsync(CancellationToken ct = default);
}
