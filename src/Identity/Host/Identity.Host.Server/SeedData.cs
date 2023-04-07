using Identity.Infrastructure.DataAccess;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Identity.Host.Server
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<AspNetIdentityDbContext>(
                options => options.UseNpgsql(connectionString));

            services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AspNetIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddOperationalDbContext(
                options =>
                {
                    options.ConfigureDbContext = db =>
                    db.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
                });

            services.AddConfigurationDbContext(
                options =>
                {
                    options.ConfigureDbContext = db =>
                    db.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
                });

            var servicesProvider = services.BuildServiceProvider();

            using var scope = servicesProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

            var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
            context.Database.Migrate();

            EnsureSeedData(context);

            var ctx = scope.ServiceProvider.GetService<AspNetIdentityDbContext>();
            ctx.Database.Migrate();
            EnsureUsers(scope);


        }


        private static void EnsureUsers(IServiceScope scope)
        {
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var angella = userMgr.FindByEmailAsync("angella").Result;
            if (angella == null)
            {
                angella = new IdentityUser
                {
                    UserName = "angella",
                    Email = "angella.freeman@email.com",
                    EmailConfirmed = true
                };

                var result = userMgr.CreateAsync(angella, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(
                    angella,
                    new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Angella Freeman"),
                        new Claim(JwtClaimTypes.GivenName, "Angella"),
                        new Claim(JwtClaimTypes.FamilyName, "Freeman"),
                        new Claim(JwtClaimTypes.WebSite, "http://angella.com"),
                        new Claim("location", "somewhere")

                    }).Result;

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
        }

        //public static void EnsureSeedData(IServiceProvider serviceProvider)
        //{
        //    using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
        //    {
        //        using (var context = scope.ServiceProvider.GetService<ConfigurationDbContext>())
        //        {
        //            EnsureSeedData(context);
        //        }
        //    }
        //}

        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            Console.WriteLine("Seeding database...");

            if (!context.Clients.Any())
            {
                Console.WriteLine("Clients being populated");
                foreach (var client in Config.Clients.ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Clients already populated");
            }

            if (!context.IdentityResources.Any())
            {
                Console.WriteLine("IdentityResources being populated");
                foreach (var resource in Config.IdentityResources.ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("IdentityResources already populated");
            }

            if (!context.ApiResources.Any())
            {
                Console.WriteLine("ApiResources being populated");
                foreach (var resource in Config.ApiResources.ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("ApiResources already populated");
            }

            if (!context.ApiScopes.Any())
            {
                Console.WriteLine("Scopes being populated");
                foreach (var resource in Config.ApiScopes.ToList())
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Scopes already populated");
            }

            Console.WriteLine("Done seeding database.");
            Console.WriteLine();
        }
    }
}
