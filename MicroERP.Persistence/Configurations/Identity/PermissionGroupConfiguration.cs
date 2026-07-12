using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroERP.Domain.Identity;

namespace MicroERP.Persistence.Configurations.Identity;

public class PermissionGroupConfiguration : IEntityTypeConfiguration<PermissionGroup>
{
    public void Configure(EntityTypeBuilder<PermissionGroup> builder)
    {
        builder.ToTable("PermissionGroups");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsSystem)
            .IsRequired();

        builder.Property(x => x.IsSystem)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(x => x.Key)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Key)
            .IsUnique();

        builder.HasMany(x => x.PermissionGroupPermissions)
            .WithOne(x => x.PermissionGroup)
            .HasForeignKey(x => x.PermissionGroupId);

        builder.HasMany(x => x.RolePermissionGroups)
            .WithOne(x => x.PermissionGroup)
            .HasForeignKey(x => x.PermissionGroupId);
    }
}