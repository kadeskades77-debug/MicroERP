using MicroERP.Domin.Entities.EmployeeAttendance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations;

public class AttendanceRecordConfiguration
    : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(
        EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("AttendanceRecords");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.Status)
            .IsRequired();



        builder.Property(x => x.Notes)
            .HasMaxLength(500);



        builder.HasOne(x => x.Employee)
            .WithMany(x => x.AttendanceRecords)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.HasPermission)
    .HasDefaultValue(true)
    .IsRequired();

        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.Date
        })
        .IsUnique();
    }
}