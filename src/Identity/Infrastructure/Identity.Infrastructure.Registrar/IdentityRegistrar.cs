using AutoMapper;
using AutoMapper.Configuration;
using Identity.Application.AppData.Repositories;
using Identity.Application.AppData.Services;
using Identity.Infrastructure.DataAccess.Contexts.Users.Repositories;
using Identity.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using IdentityServer4;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using Identity.Domain;
using Identity.Infrastructure.DataAccess;
using Identity.Infrastructure.DataAccess.Interfaces;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Identity.Application.AppData;
using FluentValidation;
using Identity.Application.AppData.Helpers;

namespace Identity.Infrastructure.Registrar
{
    public static class IdentityRegistrar
    {

        public static IServiceCollection AddServiceRegistrationModule(this IServiceCollection services)
        {
            services.AddSingleton<IDbContextOptionsConfigurator<AspNetIdentityDbContext>, AspNetIdentityDbContextConfiguration>();
            services.AddDbContext<AspNetIdentityDbContext>((Action<IServiceProvider, DbContextOptionsBuilder>)
                            ((sp, dbOptions) => sp.GetRequiredService<IDbContextOptionsConfigurator<AspNetIdentityDbContext>>()
                               .Configure((DbContextOptionsBuilder<AspNetIdentityDbContext>)dbOptions)));


            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IUserService, UserService>();

            services.AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()));

            services.AddHttpContextAccessor();

            services.AddValidatorsFromAssembly(typeof(UserLoginValidator).Assembly);

            return services;
        }

        public static IServiceCollection AddAspNetIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<Guid>>()
                .AddUserManager<UserManager<User>>()
                .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
                .AddEntityFrameworkStores<AspNetIdentityDbContext>();

            return services;
        }

        public static IServiceCollection AddIdentityServerServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            var connectionString = configuration.GetConnectionString("PostgresIdentityConfigurationDb");

            services.AddIdentityServer()
                    .AddConfigurationStore(options =>
                        options.ConfigureDbContext = b =>
                        b.UseNpgsql(connectionString))
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = b =>
                        b.UseNpgsql(connectionString);
                    })

                    .AddDeveloperSigningCredential()
                    .AddAspNetIdentity<User>();

            return services;
        }

        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
        {
            
                services.AddAuthentication()
                
                    .AddGoogle(options =>
                    {
                        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        options.ClientId = "914154221715-a1cbcguiqj50g0kaminlqarpcj5uru3i.apps.googleusercontent.com";
                        options.ClientSecret = "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";
                    });

            return services;
        }

        private static MapperConfiguration GetMapperConfiguration()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
            });
            //configuration.AssertConfigurationIsValid();

            return configuration;
        }
    }
}
