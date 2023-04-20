using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using System.Reflection;
using Identity.Infrastructure.Migrations.Contexts;

namespace Identity.Infrastructure.Migrations.Factories
{
    public class MigrationConfigurationDbContextFactory : IDesignTimeDbContextFactory<MigrationConfigurationDbContext>
    {
        public MigrationConfigurationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ConfigurationDbContext>();

            // получаем конфигурацию из файла appsettings.json
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            IConfigurationRoot config = builder.Build();

            // получаем строку подключения из файла appsettings.json
            var connectionString = config.GetConnectionString("PostgresBoardDb");
            optionsBuilder.UseNpgsql(connectionString, opts => opts
            .CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds)
            );

            var assembly = Assembly.GetExecutingAssembly();
            var defaultConnString = config.GetConnectionString("PostgresBoardDb");

            var sd = new ConfigurationStoreOptions();
            sd.ConfigureDbContext = b => b.UseNpgsql(defaultConnString, opt => opt.MigrationsAssembly("Identity.Infrastructure.Migrations"));

            return new MigrationConfigurationDbContext(optionsBuilder.Options, sd);
        }
    }
}
