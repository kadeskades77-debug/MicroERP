using MicroERP.Domin.Entities.EmployeeAttendance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroERP.Persistence.Configurations;

public class WorkScheduleConfiguration
    : IEntityTypeConfiguration<WorkSchedule>
{
    public void Configure(
        EntityTypeBuilder<WorkSchedule> builder)
    {
        builder.ToTable("WorkSchedules");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();



        builder.Property(x => x.FirstShiftStart)
            .IsRequired();


        builder.Property(x => x.FirstShiftEnd)
            .IsRequired();



        builder.Property(x => x.SecondShiftStart)
            .IsRequired(false);


        builder.Property(x => x.SecondShiftEnd)
            .IsRequired(false);



        builder.Property(x => x.LateGraceMinutes)
            .HasDefaultValue(0);


        builder.Property(x => x.EarlyLeaveGraceMinutes)
            .HasDefaultValue(0);



        builder.Property(x => x.MinimumWorkMinutes)
            .HasDefaultValue(0);



        builder.Property(x => x.IsDefault)
            .HasDefaultValue(false);



        // يسمح بجدول افتراضي واحد فقط
        builder.HasIndex(x => x.IsDefault)
            .HasFilter("[IsDefault] = 1")
            .IsUnique();
    }
}