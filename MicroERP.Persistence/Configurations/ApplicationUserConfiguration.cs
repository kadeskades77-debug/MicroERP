using MicroERP.Domin.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations
{
    public class ApplicationUserConfiguration
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.IsActive)
             .HasDefaultValue(true);
        }

        }
}
