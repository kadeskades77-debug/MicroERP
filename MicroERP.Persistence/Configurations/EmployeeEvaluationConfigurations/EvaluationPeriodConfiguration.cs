using MicroERP.Domin.Entities.EmployeeEvaluations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations.EmployeeEvaluationConfigurations
{
    public class EvaluationPeriodConfiguration
       : IEntityTypeConfiguration<EvaluationPeriod>
    {
        public void Configure(
            EntityTypeBuilder<EvaluationPeriod> builder)
        {
            builder.ToTable("EvaluationPeriods");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.StartDate)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.HasIndex(x => x.StartDate)
          .IsUnique()
          .HasFilter("[IsDeleted] = 0");
        }
    }
}
