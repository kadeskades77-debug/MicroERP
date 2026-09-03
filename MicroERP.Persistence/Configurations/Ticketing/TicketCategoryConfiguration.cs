using MicroERP.Domin.Entities.Ticketing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.Ticketing;

public class TicketCategoryConfiguration
    : IEntityTypeConfiguration<TicketCategory>
{
    public void Configure(
        EntityTypeBuilder<TicketCategory> builder)
    {
        // =========================================================
        // Table
        // =========================================================

        builder.ToTable("TicketCategories");

        // =========================================================
        // Primary Key
        // =========================================================

        builder.HasKey(x => x.Id);

        // =========================================================
        // Name
        // =========================================================

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        // =========================================================
        // Description
        // =========================================================

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        // =========================================================
        // Unique Name
        // =========================================================

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}