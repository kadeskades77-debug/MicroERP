using MicroERP.Domin.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations;

public class LeavePolicyConfiguration
    : IEntityTypeConfiguration<LeavePolicy>
{
    public void Configure(
        EntityTypeBuilder<LeavePolicy> builder)
    {
        builder.Property(x => x.DaysPerMonth)
            .HasPrecision(5, 2);


        builder.HasIndex(x => x.LeaveType)
            .IsUnique();
    }
}