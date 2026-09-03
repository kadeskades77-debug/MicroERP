using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MicroERP.Domin.Entities.EmployeeAttendance;

namespace MicroERP.Persistence.Configurations;

public class AttendancePerformanceConfiguration
    : IEntityTypeConfiguration<AttendancePerformance>
{
    public void Configure(
        EntityTypeBuilder<AttendancePerformance> builder)
    {
        builder.HasKey(x => x.Id);


        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);



        // موظف واحد له تقييم واحد لكل شهر
        builder.HasIndex(x =>
            new
            {
                x.EmployeeId,
                x.Year,
                x.Month
            })
            .IsUnique();



        builder.Property(x => x.Notes)
            .HasMaxLength(500);
    }
}