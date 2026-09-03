using MicroERP.Domin.Entities.EmployeeAttendance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations;

public class AttendanceCorrectionConfiguration
    : IEntityTypeConfiguration<AttendanceCorrection>
{
    public void Configure(
        EntityTypeBuilder<AttendanceCorrection> builder)
    {
        builder.HasKey(x => x.Id);



        builder.Property(x => x.Reason)
            .IsRequired()
            .HasMaxLength(500);



        builder.Property(x => x.RequestedByUserId)
            .IsRequired()
            .HasMaxLength(450);



        builder.Property(x => x.ApprovedByUserId)
            .HasMaxLength(450);



        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500);



        builder.HasOne(x => x.AttendanceRecord)
            .WithMany()
            .HasForeignKey(x => x.AttendanceRecordId)
            .OnDelete(DeleteBehavior.Cascade);



        builder.Property(x => x.Status)
            .HasConversion<int>();
    }
}