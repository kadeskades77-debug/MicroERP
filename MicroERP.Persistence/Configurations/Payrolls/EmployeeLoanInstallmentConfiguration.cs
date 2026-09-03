

using MicroERP.Domin.Entities.Payrolls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.Payrolls
{
    public class EmployeeLoanInstallmentConfiguration
     : IEntityTypeConfiguration<EmployeeLoanInstallment>
    {
        public void Configure(
            EntityTypeBuilder<EmployeeLoanInstallment> builder)
        {
            builder.ToTable("EmployeeLoanInstallments");

            // =========================================================
            // Primary Key
            // =========================================================

            builder.HasKey(x => x.Id);


            // =========================================================
            // Amount
            // =========================================================

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2);


            // =========================================================
            // Status
            // =========================================================

            builder.Property(x => x.Status)
                .IsRequired();


            // =========================================================
            // Notes
            // =========================================================

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);


            // =========================================================
            // Skip Reason
            // =========================================================

            builder.Property(x => x.SkipReason)
                .HasMaxLength(500);


            // =========================================================
            // Employee Loan
            // =========================================================

            builder.HasOne(x => x.EmployeeLoan)
                .WithMany(x => x.Installments)
                .HasForeignKey(x => x.EmployeeLoanId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================================================
            // Payroll Adjustment
            // =========================================================

            builder.HasOne(x => x.PayrollAdjustment)
                .WithMany()
                .HasForeignKey(x => x.PayrollAdjustmentId)
                .OnDelete(DeleteBehavior.SetNull);


            // =========================================================
            // Rescheduled Installment
            // =========================================================
            //
            // Example:
            //
            // Original:
            // #1 - August - Skipped
            //
            // Replacement:
            // #1 - September - Pending
            //     RescheduledFromInstallmentId = Original.Id
            //
            // =========================================================

            builder.HasOne(x => x.RescheduledFromInstallment)
                .WithMany(x => x.RescheduledInstallments)
                .HasForeignKey(x => x.RescheduledFromInstallmentId)
                .OnDelete(DeleteBehavior.Restrict);


            // =========================================================
            // Indexes
            // =========================================================

            builder.HasIndex(x => new
            {
                x.EmployeeLoanId,
                x.DueDate
            });


            builder.HasIndex(x => x.RescheduledFromInstallmentId);
        }
    }
}
