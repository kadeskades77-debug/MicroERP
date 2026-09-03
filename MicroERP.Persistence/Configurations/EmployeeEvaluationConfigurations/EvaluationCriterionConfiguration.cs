using MicroERP.Domin.Entities.EmployeeEvaluations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MicroERP.Persistence.Configurations.EmployeeEvaluationConfigurations
{
    public class EvaluationCriterionConfiguration
     : IEntityTypeConfiguration<EvaluationCriterion>
    {
        public void Configure(
            EntityTypeBuilder<EvaluationCriterion> builder)
        {
            builder.ToTable("EvaluationCriteria");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.MaxScore)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(x => x.Weight)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(x => x.SortOrder)
                .IsRequired();

            builder.HasOne(x => x.Template)
                .WithMany(x => x.Criteria)
                .HasForeignKey(x => x.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.TemplateId,
                x.Name
            })
          .IsUnique()
          .HasFilter("[IsDeleted] = 0");

            builder.HasIndex(x => new
            {
                x.TemplateId,
                x.SortOrder
            })
  .IsUnique()
  .HasFilter("[IsDeleted] = 0");
        }
    }
}
