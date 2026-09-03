using MicroERP.Domin.Entities.Policies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations;

public class AttendancePolicyConfiguration
    : IEntityTypeConfiguration<AttendancePolicy>
{
    public void Configure(
        EntityTypeBuilder<AttendancePolicy> builder)
    {
        builder.ToTable("AttendancePolicies");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();



        builder.Property(x => x.AbsentPenaltyPoints)
            .IsRequired();



        builder.Property(x => x.MissingCheckInPenaltyPoints)
            .IsRequired();



        builder.Property(x => x.MissingCheckOutPenaltyPoints)
            .IsRequired();



        builder.Property(x => x.LateMinutesPerPenaltyPoint)
            .IsRequired();



        builder.Property(x => x.MinimumPerformanceScore)
            .IsRequired();



        builder.Property(x => x.IsDefault)
            .IsRequired();



        // سياسة واحدة فقط تكون Default
        builder.HasIndex(x => x.IsDefault)
           .IsUnique()
          .HasFilter("[IsDefault] = 1");
    }
}