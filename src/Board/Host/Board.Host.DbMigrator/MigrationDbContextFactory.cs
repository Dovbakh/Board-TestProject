using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Host.DbMigrator
{
    public class MigrationDbContextFactory : IDesignTimeDbContextFactory<MigrationDbContext>
    {
        public MigrationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MigrationDbContext>();

            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            IConfigurationRoot config = builder.Build();

            var connectionString = config.GetConnectionString("PostgresBoardDb");
            optionsBuilder.UseNpgsql(connectionString, opts => opts
            .CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds)
            );
            return new MigrationDbContext(optionsBuilder.Options);
        }
    }
}
