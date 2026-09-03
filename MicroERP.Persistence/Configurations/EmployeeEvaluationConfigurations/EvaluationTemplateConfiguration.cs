

using MicroERP.Domin.Entities.EmployeeEvaluations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.EmployeeEvaluationConfigurations
{
    public class EvaluationTemplateConfiguration
     : IEntityTypeConfiguration<EvaluationTemplate>
    {
        public void Configure(
            EntityTypeBuilder<EvaluationTemplate> builder)
        {
            builder.ToTable("EvaluationTemplates");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.Name)
    .IsUnique()
    .HasFilter("[IsDeleted] = 0");
        }
    }
}
