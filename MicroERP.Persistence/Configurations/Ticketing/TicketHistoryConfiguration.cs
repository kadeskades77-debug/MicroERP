using MicroERP.Domin.Entities.Ticketing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.Ticketing;

public class TicketHistoryConfiguration
    : IEntityTypeConfiguration<TicketHistory>
{
    public void Configure(
        EntityTypeBuilder<TicketHistory> builder)
    {
        // =========================================================
        // Table
        // =========================================================

        builder.ToTable("TicketHistories");

        // =========================================================
        // Primary Key
        // =========================================================

        builder.HasKey(x => x.Id);

        // =========================================================
        // Action
        // =========================================================

        builder.Property(x => x.Action)
            .HasConversion<int>()
            .IsRequired();

        // =========================================================
        // Old Value
        // =========================================================

        builder.Property(x => x.OldValue)
            .HasMaxLength(2000);

        // =========================================================
        // New Value
        // =========================================================

        builder.Property(x => x.NewValue)
            .HasMaxLength(2000);

        // =========================================================
        // Notes
        // =========================================================

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        // =========================================================
        // Ticket
        // =========================================================

        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.History)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================================================
        // Indexes
        // =========================================================

        builder.HasIndex(x => new
        {
            x.TicketId,
            x.CreatedOn
        });

        builder.HasIndex(x => x.Action);
    }
}