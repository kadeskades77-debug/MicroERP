using MicroERP.Domin.Entities.Policies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.Payrolls
{
    public class PayrollPolicyConfiguration : IEntityTypeConfiguration<PayrollPolicy>
    {
        public void Configure(EntityTypeBuilder<PayrollPolicy> builder)
        {
            builder.ToTable("PayrollPolicies");


            builder.HasKey(x => x.Id);


            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();


            builder.Property(x => x.WorkingDays)
                .IsRequired();


            builder.Property(x => x.WorkingMinutesPerDay)
                .IsRequired();


            builder.Property(x => x.MonthlyFreeMinutes)
                .IsRequired();


            builder.Property(x => x.AbsentDeductionFactor)
                .HasPrecision(5, 2)
                .HasDefaultValue(1m)
                .IsRequired();


            builder.Property(x => x.EnablePartialAttendanceDeduction)
                .HasDefaultValue(false)
                .IsRequired();


            builder.Property(x => x.PartialAttendanceCalculationType)
                .HasConversion<int>()
                .IsRequired();


            builder.Property(x => x.PartialAttendancePercentage)
                .HasPrecision(5, 2);


            builder.Property(x => x.PartialAttendanceDeductionFactor)
                .HasPrecision(5, 2);


            builder.Property(x => x.IsDefault)
                .HasDefaultValue(false)
                .IsRequired();


            // سياسة واحدة فقط تكون افتراضية
            builder.HasIndex(x => x.IsDefault)
                .HasFilter("[IsDefault] = 1")
                .IsUnique();
        }
    }
}
