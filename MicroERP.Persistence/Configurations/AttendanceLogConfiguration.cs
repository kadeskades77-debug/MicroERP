using MicroERP.Domin.Entities.EmployeeAttendance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations;

public class AttendanceLogConfiguration
    : IEntityTypeConfiguration<AttendanceLog>
{
    public void Configure(
        EntityTypeBuilder<AttendanceLog> builder)
    {
        builder.ToTable("AttendanceLogs");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.DeviceEmployeeId)
            .HasMaxLength(50)
            .IsRequired();



        builder.Property(x => x.TransactionId)
            .HasMaxLength(100);



        builder.Property(x => x.RawData)
            .HasMaxLength(1000);



        // منع تكرار نفس البصمة القادمة من الجهاز
        builder.HasIndex(x => x.TransactionId)
            .IsUnique();



        // العلاقة مع الجهاز
        builder.HasOne(x => x.AttendanceDevice)
            .WithMany(x => x.AttendanceLogs)
            .HasForeignKey(x => x.AttendanceDeviceId)
            .OnDelete(DeleteBehavior.Restrict);



        // العلاقة مع الموظف (بعد المطابقة)
        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}