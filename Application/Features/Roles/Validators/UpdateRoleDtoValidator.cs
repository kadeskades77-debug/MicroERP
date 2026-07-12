using FluentValidation;
using MicroERP.Application.Common.Interfaces;
using MicroERP.Application.Features.Roles.DTOs;
using MicroERP.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace MicroERP.Application.Features.Roles.Validators
{
    public class UpdateRoleDtoValidator : AbstractValidator<UpdateRoleDto>
    {
        private readonly IApplicationDbContext _context;

        public UpdateRoleDtoValidator(IApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(100)
                .MustAsync(BeUniqueName).WithMessage("Role name already exists.");

            RuleFor(x => x.Description)
                .MaximumLength(250)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

        }

        private async Task<bool> BeUniqueName(UpdateRoleDto dto, string name, CancellationToken ct)
        {
            var normalized = name.Trim().ToUpper();

            return !await _context.Roles
                .AnyAsync(x =>
                    x.NormalizedName == normalized, ct);
        }

        private async Task<bool> BeValidPermissionGroups(
            IEnumerable<string> keys,
            CancellationToken ct)
        {
            var distinctKeys = keys
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            var count = await _context.PermissionGroups
                .CountAsync(x => distinctKeys.Contains(x.Key), ct);

            return count == distinctKeys.Count;
        }
    }
}