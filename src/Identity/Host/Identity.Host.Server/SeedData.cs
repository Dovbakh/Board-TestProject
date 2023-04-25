using Identity.Domain;
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
        public static void EnsureSeedData(ConfigurationManager configuration)
        {
            var usersDbConnectionString = configuration.GetConnectionString("PostgresIdentityUsersDb");
            var configurationDbConnectionString = configuration.GetConnectionString("PostgresIdentityConfigurationDb");

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<AspNetIdentityDbContext>(
                options => options.UseNpgsql(usersDbConnectionString));

            services
                .AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AspNetIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddOperationalDbContext(
                options =>
                {
                    options.ConfigureDbContext = db =>
                    db.UseNpgsql(configurationDbConnectionString, sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
                });

            services.AddConfigurationDbContext(
                options =>
                {
                    options.ConfigureDbContext = db =>
                    db.UseNpgsql(configurationDbConnectionString, sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
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
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var admin = userMgr.FindByEmailAsync("admin@email.com").Result;
            if (admin == null)
            {
                admin = new User
                {
                    UserName = "admin",
                    Email = "admin@email.com",
                    EmailConfirmed = true,                    
                };

                var result = userMgr.CreateAsync(admin, "Pass_123").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(
                    admin,
                    new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, "Admin"),
                        new Claim(JwtClaimTypes.GivenName, "Adminovich"),
                        new Claim(JwtClaimTypes.FamilyName, "Adminov"),
                        new Claim(JwtClaimTypes.WebSite, "http://admin.com"),
                        new Claim("location", "somewhere")

                    }).Result;

                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                var adminRole = new IdentityRole<Guid>("admin");
                result = roleMgr.CreateAsync(adminRole).Result;

                result = userMgr.AddToRoleAsync(admin, "admin").Result;
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

            //if (!context.ApiResources.Any())
            //{
            //    Console.WriteLine("ApiResources being populated");
            //    foreach (var resource in Config.ApiResources.ToList())
            //    {
            //        context.ApiResources.Add(resource.ToEntity());
            //    }
            //    context.SaveChanges();
            //}
            //else
            //{
            //    Console.WriteLine("ApiResources already populated");
            //}

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
