using FluentValidation;
using MicroERP.Application.Features.Ticketing.DTOs;
namespace MicroERP.Application.Features.Ticketing.Validators;
public class UpdateTicketCategoryValidator : AbstractValidator<UpdateTicketCategoryDto>
{
    public UpdateTicketCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).When(x => x.Name is not null);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
