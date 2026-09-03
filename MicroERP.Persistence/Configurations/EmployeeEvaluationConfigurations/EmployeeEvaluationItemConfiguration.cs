

using MicroERP.Domin.Entities.EmployeeEvaluations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.EmployeeEvaluationConfigurations
{
    public class EmployeeEvaluationItemConfiguration
    : IEntityTypeConfiguration<EmployeeEvaluationItem>
    {
        public void Configure(
            EntityTypeBuilder<EmployeeEvaluationItem> builder)
        {
            builder.ToTable("EmployeeEvaluationItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Score)
                .HasPrecision(6, 2)
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasMaxLength(2000);

            builder.HasOne(x => x.EmployeeEvaluation)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.EmployeeEvaluationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Criterion)
                .WithMany(x => x.EvaluationItems)
                .HasForeignKey(x => x.CriterionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.EmployeeEvaluationId,
                x.CriterionId
            })
            .IsUnique();
        }
    }
}
