using MicroERP.Domin.Entities.Employees;
using MicroERP.Domin.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(x => x.Phone)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Salary)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(x => x.Status)
                 .IsRequired();

            builder.Property(x => x.HireDate)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(x => x.Status)
                .HasDefaultValue(EmployeeStatus.Active);

            builder.HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.WorkSchedule)
          .WithMany()
          .HasForeignKey(x => x.WorkScheduleId)
          .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.UserId)
                .IsUnique();
        }
    }
}