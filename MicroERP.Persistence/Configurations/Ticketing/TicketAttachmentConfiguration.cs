using MicroERP.Domin.Entities.Ticketing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.Ticketing;

public class TicketAttachmentConfiguration
    : IEntityTypeConfiguration<TicketAttachment>
{
    public void Configure(
        EntityTypeBuilder<TicketAttachment> builder)
    {
        // =========================================================
        // Table
        // =========================================================

        builder.ToTable("TicketAttachments");

        // =========================================================
        // Primary Key
        // =========================================================

        builder.HasKey(x => x.Id);

        // =========================================================
        // File Name
        // =========================================================

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);

        // =========================================================
        // File Path
        // =========================================================

        builder.Property(x => x.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        // =========================================================
        // Content Type
        // =========================================================

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(150);

        // =========================================================
        // File Size
        // =========================================================

        builder.Property(x => x.FileSize)
            .IsRequired();

        // =========================================================
        // Ticket
        // =========================================================

        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================================================
        // Index
        // =========================================================

        builder.HasIndex(x => x.TicketId);
    }
}