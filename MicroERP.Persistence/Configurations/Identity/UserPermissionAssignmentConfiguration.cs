using MicroERP.Domain.Identity;
using MicroERP.Domin.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.Identity;

public class UserPermissionAssignmentConfiguration
    : IEntityTypeConfiguration<UserPermissionAssignment>
{
    public void Configure(EntityTypeBuilder<UserPermissionAssignment> builder)
    {
        builder.ToTable("UserPermissionAssignments");

        builder.HasKey(x => new
        {
            x.UserId,
            x.PermissionGroupId
        });

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserPermissionAssignments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PermissionGroup)
            .WithMany(x => x.UserPermissionAssignments)
            .HasForeignKey(x => x.PermissionGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}