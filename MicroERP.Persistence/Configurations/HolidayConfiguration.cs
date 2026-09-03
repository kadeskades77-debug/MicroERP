using MicroERP.Domin.Entities.EmployeeAttendance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations;

public class HolidayConfiguration
    : IEntityTypeConfiguration<Holiday>
{
    public void Configure(
        EntityTypeBuilder<Holiday> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.Type)
            .HasConversion<int>();
    }
}