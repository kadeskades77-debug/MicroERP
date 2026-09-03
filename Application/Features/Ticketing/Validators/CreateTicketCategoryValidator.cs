using FluentValidation;
using MicroERP.Application.Features.Ticketing.DTOs;
namespace MicroERP.Application.Features.Ticketing.Validators;
public class CreateTicketCategoryValidator : AbstractValidator<CreateTicketCategoryDto>
{
    public CreateTicketCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
