using MicroERP.Domin.Entities.Payrolls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.Payrolls;

public class PayrollConfiguration
    : IEntityTypeConfiguration<Payroll>
{
    public void Configure(EntityTypeBuilder<Payroll> builder)
    {
        builder.HasKey(x => x.Id);


        builder.Property(x => x.GrossSalary)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalAllowances)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalDeductions)
            .HasPrecision(18, 2);

        builder.Property(x => x.NetSalary)
            .HasPrecision(18, 2);


        builder.Property(x => x.Status)
            .HasConversion<int>();


        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ApprovedByUser)
    .WithMany()
    .HasForeignKey(x => x.ApprovedByUserId)
    .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(x => x.PaidByUser)
            .WithMany()
            .HasForeignKey(x => x.PaidByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.PayrollPeriodId
        })
         .IsUnique();
    }
}