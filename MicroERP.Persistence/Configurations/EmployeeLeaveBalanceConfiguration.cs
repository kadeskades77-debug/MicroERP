using MicroERP.Domin.Entities.EmployeeLeaves;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations;

public class EmployeeLeaveBalanceConfiguration
    : IEntityTypeConfiguration<EmployeeLeaveBalance>
{
    public void Configure(
        EntityTypeBuilder<EmployeeLeaveBalance> builder)
    {
        builder.HasOne(x => x.Employee)
            .WithMany(x => x.LeaveBalances)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.Property(x => x.LeaveType)
            .IsRequired();


        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.Year,
            x.LeaveType
        })
        .IsUnique();
    }
}