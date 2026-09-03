using MicroERP.Domin.Entities.EmployeeLeaves;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace MicroERP.Persistence.Configurations;

public class EmployeeLeaveConfiguration : IEntityTypeConfiguration<EmployeeLeave>
{
    public void Configure(EntityTypeBuilder<EmployeeLeave> builder)
    {
        builder.Property(x => x.Reason)
            .HasMaxLength(500);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500);

        builder.Property(x => x.ApprovedByUserId)
            .HasMaxLength(450);

        builder.Property(x => x.RejectedByUserId)
            .HasMaxLength(450);

        builder.HasOne(x => x.Employee)
            .WithMany(x => x.EmployeeLeaves)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ApprovedByUser)
    .WithMany()
    .HasForeignKey(x => x.ApprovedByUserId)
    .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(x => x.RejectedByUser)
            .WithMany()
            .HasForeignKey(x => x.RejectedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.EmployeeId);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.StartDate,
            x.EndDate
        });
    }
}