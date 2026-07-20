using FluentValidation;
using MicroERP.Application.Features.Documents.EmployeeDocuments.DTOs;

namespace MicroERP.Application.Features.Documents.EmployeeDocuments.Validators;

public class UpdateEmployeeDocumentDtoValidator
    : AbstractValidator<UpdateEmployeeDocumentDto>
{
    public UpdateEmployeeDocumentDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Name))
            .WithMessage("Document name cannot exceed 200 characters");


        RuleFor(x => x.IssueDate)
            .LessThanOrEqualTo(DateTime.Now)
            .When(x => x.IssueDate.HasValue)
            .WithMessage("Issue date cannot be in the future");


        RuleFor(x => x.ExpiryDate)
            .GreaterThan(x => x.IssueDate)
            .When(x => x.ExpiryDate.HasValue && x.IssueDate.HasValue)
            .WithMessage("Expiry date must be after issue date");


        RuleFor(x => x.File.Length)
            .LessThanOrEqualTo(10 * 1024 * 1024)
            .When(x => x.File != null)
            .WithMessage("File size cannot exceed 10 MB");


        RuleFor(x => x.File.ContentType)
            .Must(IsAllowedFileType)
            .When(x => x.File != null)
            .WithMessage("File type is not allowed");
    }


    private static bool IsAllowedFileType(string contentType)
    {
        var allowedTypes = new[]
        {
            "application/pdf",
            "image/jpeg",
            "image/png"
        };

        return allowedTypes.Contains(contentType);
    }
}