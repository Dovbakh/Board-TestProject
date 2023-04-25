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
using Identity.Infrastructure.DataAccess;

namespace Identity.Infrastructure.Migrations.Factories
{
    public class MigrationAspNetIdentityDbContextFactory : IDesignTimeDbContextFactory<MigrationAspNetIdentityDbContext>
    {
        public MigrationAspNetIdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AspNetIdentityDbContext>();

            // получаем конфигурацию из файла appsettings.json
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            IConfigurationRoot config = builder.Build();

            // получаем строку подключения из файла appsettings.json
            var connectionString = config.GetConnectionString("PostgresIdentityUsersDb");
            optionsBuilder.UseNpgsql(connectionString, opts => opts
            .CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds)
            );

            return new MigrationAspNetIdentityDbContext(optionsBuilder.Options);
        }
    }
}
