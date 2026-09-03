using FluentValidation;
using MicroERP.Application.Features.Ticketing.DTOs;
namespace MicroERP.Application.Features.Ticketing.Validators;
public class CreateTicketValidator : AbstractValidator<CreateTicketDto>
{
    public CreateTicketValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.AssignedToEmployeeId).GreaterThan(0).When(x => x.AssignedToEmployeeId.HasValue);
        RuleFor(x => x.AssignedDepartmentId).GreaterThan(0).When(x => x.AssignedDepartmentId.HasValue);
    }
}
