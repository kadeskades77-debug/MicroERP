using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroERP.Domin.Entities.Payrolls;

namespace MicroERP.Persistence.Configurations.Payrolls;

public class PayrollAdjustmentConfiguration
    : IEntityTypeConfiguration<PayrollAdjustment>
{
    public void Configure(
        EntityTypeBuilder<PayrollAdjustment> builder)
    {
        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId);

        builder.HasOne(x => x.PayrollPeriod)
            .WithMany()
            .HasForeignKey(x => x.PayrollPeriodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}