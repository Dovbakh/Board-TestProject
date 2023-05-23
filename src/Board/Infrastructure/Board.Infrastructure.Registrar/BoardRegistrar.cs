using AutoMapper;
using Board.Application.AppData.Contexts.Adverts.Services;
using Board.Application.AppData.Contexts.Categories.Repositories;
using Board.Application.AppData.Contexts.Categories.Services;
using Board.Application.AppData.Contexts.Comments.Repositories;
using Board.Application.AppData.Contexts.Comments.Services;
using Board.Application.AppData.Contexts.AdvertImages.Repositories;
using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Infrastructure.DataAccess;
using Board.Infrastructure.DataAccess.Contexts.Categories.Repositories;
using Board.Infrastructure.DataAccess.Contexts.Comments.Repositories;
using Board.Infrastructure.DataAccess.Contexts.AdvertImages.Repositories;
using Board.Infrastructure.DataAccess.Contexts.Adverts.Repositories;
using Board.Infrastructure.DataAccess.Interfaces;
using Board.Infrastructure.Registrar.MapProfiles;
using Board.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Board.Application.AppData.Contexts.Users.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Board.Domain;
using Microsoft.OpenApi.Models;
using Identity.Clients.Users;
using Microsoft.Extensions.Options;
using Board.Contracts.Options;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Identity.Infrastructure.Registrar;
using Board.Application.AppData.Contexts.Users;
using FluentValidation;
using Board.Application.AppData.Contexts.Users.Helpers;
using Board.Application.AppData.Contexts.Images.Services;
using FileStorage.Clients.Contexts.Images;
using FileStorage.Infrastructure.Registrar;
using SixLabors.ImageSharp;
using Board.Application.AppData.Contexts.Notifications.Services;
using MassTransit;
using RedLockNet.SERedis;
using RedLockNet;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Templates;
using Board.Application.AppData.Contexts.AdvertViews.Services;
using Board.Application.AppData.Contexts.AdvertFavorites.Services;
using Board.Application.AppData.Contexts.AdvertViews.Repositories;
using Board.Infrastructure.DataAccess.Contexts.AdvertViews.Repositories;
using Board.Application.AppData.Contexts.AdvertFavorites.Repositories;
using Board.Infrastructure.DataAccess.Contexts.AdvertFavorites.Repositories;
using IdentityModel.Client;
using Board.Contracts.Options;

namespace Board.Infrastructure.Registrar
{
    /// <summary>
    /// Регистратор сервисов для микросервиса Board
    /// </summary>
    public static class BoardRegistrar
    {
        /// <summary>
        /// Регистрация application-сервисов, репозиториев и вспомогательных сервисов.
        /// </summary>
        public static IServiceCollection AddServiceRegistrationModule(this IServiceCollection services, IConfiguration configuration)
        {
            #region Регистрация сервисов работы с БД
            services.AddSingleton<IDbContextOptionsConfigurator<BoardDbContext>, BoardDbContextConfiguration>();
            services.AddDbContext<BoardDbContext>((Action<IServiceProvider, DbContextOptionsBuilder>)
                ((sp, dbOptions) => sp.GetRequiredService<IDbContextOptionsConfigurator<BoardDbContext>>()
                   .Configure((DbContextOptionsBuilder<BoardDbContext>)dbOptions)));
            services.AddScoped((Func<IServiceProvider, DbContext>)(sp => sp.GetRequiredService<BoardDbContext>()));
            #endregion

            #region Регистрация репозиториев
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<IAdvertRepository, AdvertRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IAdvertImageRepository, AdvertImageRepository>();
            services.AddScoped<IAdvertViewRepository, AdvertViewRepository>();
            services.AddScoped<IAdvertFavoriteRepository, AdvertFavoriteRepository>();

            #endregion

            #region Регистрация application-сервисов
            services.AddScoped<IAdvertService, AdvertService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAdvertViewService, AdvertViewService>();
            services.AddScoped<IAdvertFavoriteService, AdvertFavoriteService>();
            services.AddScoped<IImageService, ImageService>();
            #endregion

            #region Регистрация вспомогательных сервисов
            services.AddHttpContextAccessor();
            services.AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()));
            services.AddValidatorsFromAssembly(typeof(UserLoginValidator).Assembly);
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetSection("RedisCache").GetRequiredSection("Host").Value;
                options.InstanceName = configuration.GetSection("RedisCache").GetRequiredSection("InstanceName").Value;

            });
            services.AddSingleton<IDistributedLockFactory, RedLockFactory>(x =>
                RedLockFactory.Create(new List<RedLockMultiplexer>
                {
                    ConnectionMultiplexer.Connect(configuration["RedisCache:HostPort"])
                }));
            services.AddMassTransit(mt => mt.AddMassTransit(x => {
                x.UsingRabbitMq((cntxt, cfg) => {
                    cfg.Host(configuration["RabbitMQ:Address"], "/", c => {
                        c.Username(configuration["RabbitMQ:Username"]);
                        c.Password(configuration["RabbitMQ:Password"]);
                    });
                });
            }));
            #endregion

            #region Регистрация опций
            services.AddOptions<CommentAddLockOptions>()
                .BindConfiguration("RedLock:CommentAddLockOptions")
                .ValidateOnStart();
            services.AddOptions<AdvertFavoriteAddLockOptions>()
                .BindConfiguration("RedLock:AdvertFavoriteAddLockOptions")
                .ValidateOnStart();
            services.AddOptions<AdvertOptions>()
                .BindConfiguration("Adverts")
                .ValidateOnStart();
            services.AddOptions<AdvertFavoriteOptions>()
                .BindConfiguration("AdvertFavorites")
                .ValidateOnStart();           
            services.AddOptions<CommentOptions>()
                .BindConfiguration("Comments")
                .ValidateOnStart();
            services.AddOptions<CategoryOptions>()
                .BindConfiguration("Categories")
                .ValidateOnStart();
            services.AddOptions<Contracts.Options.UserOptions>()
                .BindConfiguration("Users")
                .ValidateOnStart();
            services.AddOptions<CookieOptions>()
                .BindConfiguration("Cookie")
                .ValidateOnStart();
            services.AddOptions<IdentityClientOptions>()
                .BindConfiguration("IdentityServer")
                .ValidateOnStart();
            #endregion

           
            return services;
        }

        /// <summary>
        /// Регистрация сервисов для работы с аутентификацией.
        /// </summary>
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = configuration["IdentityServer:Address"];
                    options.RequireHttpsMetadata = false;
                    
                });

            return services;
        }

        /// <summary>
        /// Регистрация сервисов для работы с авторизацией.
        /// </summary>
        public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "Board.Host.Api");
                });
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireRole("admin");
                });

            });

            return services;
        }

        /// <summary>
        /// Регистрация сервисов для работы с Redis.
        /// </summary>
        public static IServiceCollection AddRedisServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["RedisCache:Host"];
                options.InstanceName = configuration["RedisCache:InstanceName"];
            });

            return services;
        }

        /// <summary>
        /// Регистрация сервисов для работы со Swagger.
        /// </summary>
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.FullName.Replace("+", "_"));
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Solarvito Api", Version = "V1" });
                options.IncludeXmlComments(Path.Combine(Path.Combine(AppContext.BaseDirectory, "Documentation.xml")));
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.  
                        Enter 'Bearer' [space] and then your token in the text input below.
                        Example: 'Bearer secretKey'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            }).AddSwaggerGenNewtonsoftSupport();

            return services;
        }

        /// <summary>
        /// Регистрация сервисов для работы с HTTP-клиентами.
        /// </summary>
        public static IServiceCollection AddHttpClients(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddScoped<IUserClient, UserClient>();
            services.AddScoped<IImageClient, ImageClient>();   

            services.AddHttpClient();
            services.AddAccessTokenManagement(options =>
            {
                options.Client.Clients.Add("Identity.Host.Server", new ClientCredentialsTokenRequest
                {
                    Address = configuration.GetSection("IdentityServer").GetSection("GetTokenAddress").Value,
                    ClientId = configuration.GetSection("IdentityServer").GetSection("InternalClientCredentials").GetSection("Id").Value,
                    ClientSecret = configuration.GetSection("IdentityServer").GetSection("InternalClientCredentials").GetSection("Secret").Value,
                    Scope = configuration.GetSection("IdentityServer").GetSection("InternalClientCredentials").GetSection("Scope").Value
                });
            });

            services.AddClientAccessTokenClient("UserClient", configureClient: client =>
            {
                client.BaseAddress = new Uri(configuration.GetSection("UserApiClientOptions").GetSection("BasePath").Value);
            });

            services.AddClientAccessTokenClient("FileClient", configureClient: client =>
            {
                client.BaseAddress = new Uri(configuration.GetSection("FileApiClientOptions").GetSection("BasePath").Value);
            });

            return services;
        }

        /// <summary>
        /// Регистрация сервисов для работы с логгером.
        /// </summary>
        public static ConfigureHostBuilder AddCustomLogger(this ConfigureHostBuilder hostBuilder, ConfigurationManager configuration)
        {
            hostBuilder.UseSerilog((context, services, cfg) =>
                cfg.ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(configuration["Seq:Address"]));

            return hostBuilder;
        }

        /// <summary>
        /// Получить конфигурацию AutoMapper.
        /// </summary>
        private static MapperConfiguration GetMapperConfiguration()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(typeof(BoardRegistrar), typeof(IdentityRegistrar), typeof(FileStorageRegistrar));
            });
            configuration.AssertConfigurationIsValid();

            return configuration;
        }
    }
}
