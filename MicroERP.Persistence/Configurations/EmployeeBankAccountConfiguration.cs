

namespace MicroERP.Persistence.Configurations
{
   
    using MicroERP.Domin.Entities.Payrolls;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class EmployeeBankAccountConfiguration :
        IEntityTypeConfiguration<EmployeeBankAccount>
    {
        public void Configure(
            EntityTypeBuilder<EmployeeBankAccount> builder)
        {
            builder.ToTable("EmployeeBankAccounts");


            builder.HasKey(x => x.Id);



            builder.Property(x => x.BankName)
                .HasMaxLength(100)
                .IsRequired();



            builder.Property(x => x.BranchName)
                .HasMaxLength(100);



            builder.Property(x => x.AccountNumber)
                .HasMaxLength(50)
                .IsRequired();



            builder.Property(x => x.IBAN)
                .HasMaxLength(50);



            builder.HasOne(x => x.Employee)
                .WithMany(x => x.BankAccounts)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);



            builder.HasIndex(x =>
                new
                {
                    x.EmployeeId,
                    x.IsPrimary
                })
                .HasDatabaseName(
                    "IX_EmployeeBankAccounts_Primary");



            builder.HasQueryFilter(
                x => !x.IsDeleted);
        }
    }
}
