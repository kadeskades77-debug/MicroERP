using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroERP.Domain.Identity;

namespace MicroERP.Persistence.Configurations.Identity;

public class PermissionGroupPermissionConfiguration : IEntityTypeConfiguration<PermissionGroupPermission>
{
    public void Configure(EntityTypeBuilder<PermissionGroupPermission> builder)
    {
        builder.ToTable("PermissionGroupPermissions");

        builder.HasKey(x => new { x.PermissionGroupId, x.PermissionId });

        builder.HasOne(x => x.PermissionGroup)
            .WithMany(x => x.PermissionGroupPermissions)
            .HasForeignKey(x => x.PermissionGroupId);

        builder.HasOne(x => x.Permission)
            .WithMany(x => x.PermissionGroupPermissions)
            .HasForeignKey(x => x.PermissionId);
    }
}