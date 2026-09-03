using MicroERP.Domin.Entities.Ticketing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.Ticketing;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        // =========================================================
        // Table
        // =========================================================

        builder.ToTable("Tickets");

        // =========================================================
        // Primary Key
        // =========================================================

        builder.HasKey(x => x.Id);

        // =========================================================
        // Ticket Number
        // =========================================================

        builder.Property(x => x.TicketNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.TicketNumber)
            .IsUnique();

        // =========================================================
        // Title
        // =========================================================

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        // =========================================================
        // Description
        // =========================================================

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(5000);

        // =========================================================
        // Status
        // =========================================================

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        // =========================================================
        // Priority
        // =========================================================

        builder.Property(x => x.Priority)
            .HasConversion<int>()
            .IsRequired();

        // =========================================================
        // Category
        // =========================================================

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Tickets)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================================================
        // Assigned Employee
        // =========================================================

        builder.HasOne(x => x.AssignedToEmployee)
            .WithMany()
            .HasForeignKey(x => x.AssignedToEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================================================
        // Assigned Department
        // =========================================================

        builder.HasOne(x => x.AssignedDepartment)
            .WithMany()
            .HasForeignKey(x => x.AssignedDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================================================
        // Indexes
        // =========================================================

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.Priority);

        builder.HasIndex(x => x.CategoryId);

        builder.HasIndex(x => x.AssignedToEmployeeId);

        builder.HasIndex(x => x.AssignedDepartmentId);

        builder.HasIndex(x => new
        {
            x.Status,
            x.Priority
        });

        builder.HasIndex(x => new
        {
            x.CreatedBy,
            x.CreatedOn
        });
    }
}