using FluentValidation;
using MicroERP.Application.Features.Ticketing.DTOs;
namespace MicroERP.Application.Features.Ticketing.Validators;
public class AssignTicketValidator : AbstractValidator<AssignTicketDto>
{
    public AssignTicketValidator()
    {
        RuleFor(x => x.TicketId).GreaterThan(0);
        RuleFor(x => x.EmployeeId).GreaterThan(0);
       
    }
}
