

using MicroERP.Domin.Entities.Payrolls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.Payrolls
{
    public class EmployeeLoanConfiguration
     : IEntityTypeConfiguration<EmployeeLoan>
    {
        public void Configure(
            EntityTypeBuilder<EmployeeLoan> builder)
        {
            builder.ToTable("EmployeeLoans");

            // =========================================================
            // Primary Key
            // =========================================================

            builder.HasKey(x => x.Id);


            // =========================================================
            // Basic Properties
            // =========================================================

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);


            // =========================================================
            // Money
            // =========================================================

            builder.Property(x => x.TotalAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.InstallmentAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.PaidAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.RemainingAmount)
                .HasPrecision(18, 2);


            // =========================================================
            // Status
            // =========================================================

            builder.Property(x => x.Status)
                .IsRequired();


            // =========================================================
            // Employee
            // =========================================================

            builder.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================================================
            // Cancellation User
            // =========================================================

            builder.HasOne(x => x.CancelledByUser)
                .WithMany()
                .HasForeignKey(x => x.CancelledByUserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(x => x.CancellationReason)
                .HasMaxLength(1000);


            // =========================================================
            // Suspension User
            // =========================================================

            builder.HasOne(x => x.SuspendedByUser)
                .WithMany()
                .HasForeignKey(x => x.SuspendedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Property(x => x.SuspensionReason)
                .HasMaxLength(1000);


            // =========================================================
            // Indexes
            // =========================================================

            builder.HasIndex(x => new
            {
                x.EmployeeId,
                x.Status
            });

            builder.HasIndex(x => x.CancelledByUserId);

            builder.HasIndex(x => x.SuspendedByUserId);
        }
    }
}
