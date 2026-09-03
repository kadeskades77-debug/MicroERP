
using MicroERP.Domin.Entities.Payrolls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.Payrolls;

public class PayrollItemConfiguration
    : IEntityTypeConfiguration<PayrollItem>
{
    public void Configure(EntityTypeBuilder<PayrollItem> builder)
    {
        builder.HasKey(x => x.Id);


        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);


        builder.HasOne(x => x.Payroll)
            .WithMany(x => x.PayrollItems)
            .HasForeignKey(x => x.PayrollId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne(x => x.SalaryComponent)
            .WithMany()
            .HasForeignKey(x => x.SalaryComponentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}