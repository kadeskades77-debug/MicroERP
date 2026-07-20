using MicroERP.Domin.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations;

public class EmployeeSpecialLeaveConfiguration
    : IEntityTypeConfiguration<EmployeeSpecialLeave>
{
    public void Configure(
        EntityTypeBuilder<EmployeeSpecialLeave> builder)
    {
        builder.ToTable("EmployeeSpecialLeaves");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.Type)
            .IsRequired();



        builder.Property(x => x.Status)
            .IsRequired();



        builder.Property(x => x.Reason)
            .HasMaxLength(500);


        builder.Property(x => x.RejectionReason)
    .HasMaxLength(500);

        builder.HasOne(x => x.Employee)
            .WithMany()
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

        builder.HasIndex(x => new
        {
            x.EmployeeId,
            x.Type,
            x.Status
        });
    }
}