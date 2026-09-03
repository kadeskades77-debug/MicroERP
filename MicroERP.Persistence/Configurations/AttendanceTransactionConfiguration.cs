using MicroERP.Domin.Entities.EmployeeAttendance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations;

public class AttendanceTransactionConfiguration
    : IEntityTypeConfiguration<AttendanceTransaction>
{
    public void Configure(
        EntityTypeBuilder<AttendanceTransaction> builder)
    {
        builder.ToTable("AttendanceTransactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TransactionTime)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();


        builder.HasOne(x => x.AttendanceRecord)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.AttendanceRecordId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne(x => x.AttendanceDevice)
            .WithMany()
            .HasForeignKey(x => x.AttendanceDeviceId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(x => x.AttendanceLog)
    .WithOne()
    .HasForeignKey<AttendanceTransaction>(x => x.AttendanceLogId)
    .IsRequired(false)
    .OnDelete(DeleteBehavior.Restrict);


        // يمنع تحويل نفس AttendanceLog أكثر من مرة
        builder.HasIndex(x => x.AttendanceLogId)
            .IsUnique();


        builder.HasIndex(x => new
        {
            x.AttendanceRecordId,
            x.TransactionTime
        });
    }
}