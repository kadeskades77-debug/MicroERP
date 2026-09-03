using FluentValidation;
using MicroERP.Application.Features.Ticketing.DTOs;
namespace MicroERP.Application.Features.Ticketing.Validators;
public class ChangeTicketStatusValidator : AbstractValidator<ChangeTicketStatusDto>
{
    public ChangeTicketStatusValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0);

        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
