
using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations;

public class PositionConfiguration
    : IEntityTypeConfiguration<Position>
{
    public void Configure(
        EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("Positions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.NameAr)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.NameEn)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.AssignmentType)
            .IsRequired()
            .HasDefaultValue(PositionAssignmentType.Multiple);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasIndex(x => x.NameAr);

        builder.HasIndex(x => x.NameEn);

        builder.HasMany(x => x.Employees)
            .WithOne(x => x.Position)
            .HasForeignKey(x => x.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}