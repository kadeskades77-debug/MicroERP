using FluentValidation;
using MicroERP.Application.Features.Ticketing.DTOs;
namespace MicroERP.Application.Features.Ticketing.Validators;
public class AddTicketAttachmentValidator : AbstractValidator<AddTicketAttachmentDto>
{
    public AddTicketAttachmentValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0);
        RuleFor(x => x.File).NotNull();
        RuleFor(x => x.File.Length).GreaterThan(0).When(x => x.File is not null);
    }
}
