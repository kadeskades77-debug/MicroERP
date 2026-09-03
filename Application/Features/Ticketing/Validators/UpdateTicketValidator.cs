using FluentValidation;
using MicroERP.Application.Features.Ticketing.DTOs;
namespace MicroERP.Application.Features.Ticketing.Validators;
public class UpdateTicketValidator : AbstractValidator<UpdateTicketDto>
{
    public UpdateTicketValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200).When(x => x.Title is not null);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000).When(x => x.Description is not null);
        RuleFor(x => x.CategoryId).GreaterThan(0).When(x => x.CategoryId.HasValue);
  
    }
}
