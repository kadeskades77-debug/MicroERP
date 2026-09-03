using FluentValidation;
using MicroERP.Application.Features.Ticketing.DTOs;
namespace MicroERP.Application.Features.Ticketing.Validators;
public class AddTicketCommentValidator : AbstractValidator<AddTicketCommentDto>
{
    public AddTicketCommentValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0);
        RuleFor(x => x.Comment).NotEmpty().MaximumLength(5000);
    }
}
