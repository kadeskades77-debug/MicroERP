using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MicroERP.Persistence
{
    public class ApplicationDbContextFactory
        : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(
    Path.Combine(
        Directory.GetCurrentDirectory(),
        "../Micro ERP"))
                .AddJsonFile(
                    "appsettings.json")
                .Build();


            var optionsBuilder =
                new DbContextOptionsBuilder<ApplicationDbContext>();


            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                options =>
                {
                    options.MigrationsAssembly(
                        "MicroERP.Persistence");
                });


            return new ApplicationDbContext(
                optionsBuilder.Options);
        }
    }
}
