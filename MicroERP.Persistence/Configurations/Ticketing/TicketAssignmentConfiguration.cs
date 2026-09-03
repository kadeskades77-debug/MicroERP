using MicroERP.Domin.Entities.Ticketing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.Ticketing;

public class TicketAssignmentConfiguration
    : IEntityTypeConfiguration<TicketAssignment>
{
    public void Configure(
        EntityTypeBuilder<TicketAssignment> builder)
    {
        // =========================================================
        // Table
        // =========================================================

        builder.ToTable("TicketAssignments");

        // =========================================================
        // Primary Key
        // =========================================================

        builder.HasKey(x => x.Id);

        // =========================================================
        // Is Current
        // =========================================================

        builder.Property(x => x.IsCurrent)
            .IsRequired();

        // =========================================================
        // Ticket
        // =========================================================

        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.Assignments)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================================================
        // Employee
        // =========================================================

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================================================
        // Department
        // =========================================================

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================================================
        // Indexes
        // =========================================================

        builder.HasIndex(x => new
        {
            x.TicketId,
            x.IsCurrent
        });

        builder.HasIndex(x => x.EmployeeId);

        builder.HasIndex(x => x.DepartmentId);
    }
}