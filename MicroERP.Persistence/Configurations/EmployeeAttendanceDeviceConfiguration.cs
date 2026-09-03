using MicroERP.Domin.Entities.EmployeeAttendance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations;

public class EmployeeAttendanceDeviceConfiguration
    : IEntityTypeConfiguration<EmployeeAttendanceDevice>
{
    public void Configure(
        EntityTypeBuilder<EmployeeAttendanceDevice> builder)
    {
        builder.ToTable("EmployeeAttendanceDevices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DeviceEmployeeId)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.AttendanceDevices)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AttendanceDevice)
            .WithMany(x => x.EmployeeMappings)
            .HasForeignKey(x => x.AttendanceDeviceId)
            .OnDelete(DeleteBehavior.Restrict);

        // يمنع تكرار نفس رقم الموظف في نفس الجهاز
        builder.HasIndex(x => new
        {
            x.AttendanceDeviceId,
            x.DeviceEmployeeId
        })
        .IsUnique();

        // يمنع ربط الموظف أكثر من مرة بنفس الجهاز
        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.AttendanceDeviceId
        })
        .IsUnique();
    }
}