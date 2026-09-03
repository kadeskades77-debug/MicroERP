using MicroERP.Domin.Entities.Policies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations;

public class OvertimePolicyConfiguration
    : IEntityTypeConfiguration<OvertimePolicy>
{
    public void Configure(
        EntityTypeBuilder<OvertimePolicy> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();


        builder.Property(x => x.NormalMultiplier)
            .HasColumnType("decimal(5,2)");


        builder.Property(x => x.WeekendMultiplier)
            .HasColumnType("decimal(5,2)");


        builder.Property(x => x.HolidayMultiplier)
            .HasColumnType("decimal(5,2)");


        builder.HasIndex(x => x.IsDefault)
            .IsUnique()
            .HasFilter("[IsDefault] = 1");
    }
}