

using MicroERP.Domin.Entities.EmployeeEvaluations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.EmployeeEvaluationConfigurations
{
    public class EmployeeEvaluationConfiguration
    : IEntityTypeConfiguration<EmployeeEvaluation>
    {
        public void Configure(
            EntityTypeBuilder<EmployeeEvaluation> builder)
        {
            builder.ToTable("EmployeeEvaluations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EvaluatorId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(x => x.TotalScore)
                .HasPrecision(6, 2)
                .IsRequired();

            builder.Property(x => x.FinalRate)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Evaluator)
                .WithMany()
                .HasForeignKey(x => x.EvaluatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Period)
                .WithMany(x => x.EmployeeEvaluations)
                .HasForeignKey(x => x.PeriodId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Template)
                .WithMany(x => x.EmployeeEvaluations)
                .HasForeignKey(x => x.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.EmployeeId,
                x.PeriodId
            })
        .IsUnique()
        .HasFilter("[IsDeleted] = 0");
        }
    }
}
