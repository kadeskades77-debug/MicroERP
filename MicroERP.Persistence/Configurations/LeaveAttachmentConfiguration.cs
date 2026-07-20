using MicroERP.Domin.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations
{
    public class LeaveAttachmentConfiguration
      : IEntityTypeConfiguration<LeaveAttachment>
    {
        public void Configure(EntityTypeBuilder<LeaveAttachment> builder)
        {
            builder.Property(x => x.FileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.FilePath)
                .HasMaxLength(500)
                .IsRequired();


            builder.HasOne(x => x.EmployeeLeave)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.EmployeeLeaveId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(x => x.EmployeeSpecialLeave)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.EmployeeSpecialLeaveId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.EmployeeLeaveId);

            builder.HasIndex(x => x.EmployeeSpecialLeaveId);
        }
    }
}
