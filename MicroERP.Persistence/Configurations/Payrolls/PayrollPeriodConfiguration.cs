using MicroERP.Domin.Entities.Payrolls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.Payrolls;

public class PayrollPeriodConfiguration
    : IEntityTypeConfiguration<PayrollPeriod>
{
    public void Configure(EntityTypeBuilder<PayrollPeriod> builder)
    {
        builder.HasKey(x => x.Id);


        builder.HasIndex(x => new
        {
            x.Year,
            x.Month
        })
        .IsUnique();


        builder.Property(x => x.Status)
            .HasConversion<int>();


        builder.HasMany(x => x.Payrolls)
            .WithOne(x => x.PayrollPeriod)
            .HasForeignKey(x => x.PayrollPeriodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}