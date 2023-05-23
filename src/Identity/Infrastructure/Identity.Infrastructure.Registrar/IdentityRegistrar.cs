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
using RedLockNet.SERedis.Configuration;
using RedLockNet.SERedis;
using RedLockNet;
using StackExchange.Redis;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Identity.Contracts.Options;

namespace Identity.Infrastructure.Registrar
{
    public static class IdentityRegistrar
    {

        public static IServiceCollection AddServiceRegistrationModule(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
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

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetSection("RedisCache").GetRequiredSection("HostParams").Value;
                options.InstanceName = configuration.GetSection("RedisCache").GetRequiredSection("InstanceName").Value;

            });

            services.AddScoped<ICacheRepository, CacheRepository>();

            services.AddSingleton<IDistributedLockFactory, RedLockFactory>(x =>
                RedLockFactory.Create(new List<RedLockMultiplexer>
                {
                    ConnectionMultiplexer.Connect(configuration["RedisCache:HostPort"])
                }));


            services.AddOptions<UserRegisterLockOptions>()
                .BindConfiguration("RedLock:UserRegisterLockOptions")
                .ValidateOnStart();
            services.AddOptions<Contracts.Options.UserOptions>()
                .BindConfiguration("Users")
                .ValidateOnStart();

            return services;
        }

        public static IServiceCollection AddAspNetIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole<Guid>>()
                .AddUserManager<UserManager<User>>()
                .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
                .AddEntityFrameworkStores<AspNetIdentityDbContext>()
                .AddDefaultTokenProviders();

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
                    .AddAspNetIdentity<User>()
                    .AddProfileService<ProfileService>();

            return services;
        }

        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)            
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = configuration["IdentityServer:Address"];
                    options.RequireHttpsMetadata = false;

                })
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = configuration["GoogleAuth:ClientId"];
                    options.ClientSecret = configuration["GoogleAuth:ClientSecret"];
                }); 

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "Identity.Host.Server");
                });
            });             
                    
            return services;
        }
        public static ConfigureHostBuilder AddCustomLogger(this ConfigureHostBuilder hostBuilder, ConfigurationManager configuration)
        {
            hostBuilder.UseSerilog((context, services, cfg) =>
                cfg.ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(configuration["Seq:Address"]));

            return hostBuilder;
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
