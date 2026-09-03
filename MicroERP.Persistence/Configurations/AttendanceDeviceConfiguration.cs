using MicroERP.Domin.Entities.EmployeeAttendance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations;

public class AttendanceDeviceConfiguration
    : IEntityTypeConfiguration<AttendanceDevice>
{
    public void Configure(
        EntityTypeBuilder<AttendanceDevice> builder)
    {
        builder.ToTable("AttendanceDevices");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();



        builder.Property(x => x.DeviceCode)
            .HasMaxLength(50)
            .IsRequired();



        builder.HasIndex(x => x.DeviceCode)
            .IsUnique();



        builder.Property(x => x.IpAddress)
            .HasMaxLength(50);



        builder.Property(x => x.Location)
            .HasMaxLength(200);



        builder.HasMany(x => x.AttendanceLogs)
            .WithOne(x => x.AttendanceDevice)
            .HasForeignKey(x => x.AttendanceDeviceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}