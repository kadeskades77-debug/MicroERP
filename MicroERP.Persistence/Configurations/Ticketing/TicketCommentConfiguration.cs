using MicroERP.Domin.Entities.Ticketing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.Ticketing;

public class TicketCommentConfiguration
    : IEntityTypeConfiguration<TicketComment>
{
    public void Configure(
        EntityTypeBuilder<TicketComment> builder)
    {
        // =========================================================
        // Table
        // =========================================================

        builder.ToTable("TicketComments");

        // =========================================================
        // Primary Key
        // =========================================================

        builder.HasKey(x => x.Id);

        // =========================================================
        // Comment
        // =========================================================

        builder.Property(x => x.Comment)
            .IsRequired()
            .HasMaxLength(5000);

        // =========================================================
        // Ticket
        // =========================================================

        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.Comments)
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
        // Indexes
        // =========================================================

        builder.HasIndex(x => new
        {
            x.TicketId,
            x.CreatedOn
        });

        builder.HasIndex(x => x.EmployeeId);
    }
}