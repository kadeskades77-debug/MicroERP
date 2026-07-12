using MicroERP.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.Identity;

public class RolePermissionGroupConfiguration
    : IEntityTypeConfiguration<RolePermissionGroup>
{
    public void Configure(EntityTypeBuilder<RolePermissionGroup> builder)
    {
        builder.ToTable("RolePermissionGroups");

        builder.HasKey(x => new
        {
            x.RoleId,
            x.PermissionGroupId
        });


        builder.Property(x => x.RoleId)
            .IsRequired();


        builder.HasOne(x => x.Role)
            .WithMany(x => x.RolePermissionGroups)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne(x => x.PermissionGroup)
            .WithMany(x => x.RolePermissionGroups)
            .HasForeignKey(x => x.PermissionGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}