using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroERP.Domin.Entities.EmployeeAttendance;

namespace MicroERP.Persistence.Configurations;

public class EmployeeOvertimeConfiguration
    : IEntityTypeConfiguration<EmployeeOvertime>
{
    public void Configure(
        EntityTypeBuilder<EmployeeOvertime> builder)
    {
        builder.Property(x => x.HourlyRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Multiplier)
            .HasColumnType("decimal(5,2)");

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Reason)
            .HasMaxLength(1000);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ApprovedByUser)
            .WithMany()
            .HasForeignKey(x => x.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RejectedByUser)
            .WithMany()
            .HasForeignKey(x => x.RejectedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PayrollItem)
            .WithMany()
            .HasForeignKey(x => x.PayrollItemId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.CancelledByUser)
    .WithMany()
    .HasForeignKey(x => x.CancelledByUserId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(1000);

        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.StartDateTime
        });

        builder.HasIndex(x => new
        {
            x.Status,
            x.IsPaid
        });
    }
}