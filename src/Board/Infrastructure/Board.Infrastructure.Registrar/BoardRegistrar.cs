﻿using AutoMapper;
using Board.Application.AppData.Contexts.Adverts.Services;
using Board.Application.AppData.Contexts.Categories.Repositories;
using Board.Application.AppData.Contexts.Categories.Services;
using Board.Application.AppData.Contexts.Comments.Repositories;
using Board.Application.AppData.Contexts.Comments.Services;
using Board.Application.AppData.Contexts.AdvertImages.Repositories;
using Board.Application.AppData.Contexts.Adverts.Repositories;
using Board.Application.AppData.Contexts.Users.Repositories;
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
using Board.Infrastructure.Registrar.Options;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Identity.Infrastructure.Registrar;
using Board.Application.AppData.Contexts.Users;
using FluentValidation;
using Board.Application.AppData.Contexts.Users.Helpers;
using Board.Application.AppData.Contexts.Files.Services;
using FileStorage.Clients.Contexts.Files;
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

namespace Board.Infrastructure.Registrar
{
    public static class BoardRegistrar
    {
        public static IServiceCollection AddServiceRegistrationModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Регистрация сервисов работы с БД
            services.AddSingleton<IDbContextOptionsConfigurator<BoardDbContext>, BoardDbContextConfiguration>();
            services.AddDbContext<BoardDbContext>((Action<IServiceProvider, DbContextOptionsBuilder>)
                ((sp, dbOptions) => sp.GetRequiredService<IDbContextOptionsConfigurator<BoardDbContext>>()
                   .Configure((DbContextOptionsBuilder<BoardDbContext>)dbOptions)));
            services.AddScoped((Func<IServiceProvider, DbContext>)(sp => sp.GetRequiredService<BoardDbContext>()));


            //// Регистрация сервисов работы с обьектным хранилищем
            //services.AddScoped<IObjectStorage, MinioStorage>();

            #region Регистрация репозиториев
            // Регистрация репозиториев
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            //services.AddScoped(typeof(ICachedRepository<>), typeof(CachedRepository<>));
            services.AddScoped<IAdvertRepository, AdvertRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            //services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IAdvertImageRepository, AdvertImageRepository>();
            #endregion

            //// Регистрация application-сервисов
            services.AddScoped<IAdvertService, AdvertService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IFileService, FileService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<INotificationService, NotificationService>();
            //services.AddScoped<INotifierService, EmailService>();

            //// Регистрация вспомогательных сервисов
            //services.AddSingleton<IDateTimeService, DateTimeService>();
            //services.AddTransient(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
            //services.AddScoped<IClaimsAccessor, HttpContextClaimsAccessor>();
            //services.AddTransient<IValidator<UserRegisterDto>, UserRegisterValidator>();
            //services.AddTransient<IValidator<UserLoginDto>, UserLoginValidator>();
            //services.AddTransient<IValidator<UserChangePasswordDto>, UserChangePasswordValidator>();
            //services.AddTransient<IValidator<UserChangeEmailDto>, UserChangeEmailValidator>();
            //services.AddTransient<IValidator<UserUpdateRequestDto>, UserUpdateValidator>();
            //services.AddTransient<IValidator<UserEmailDto>, UserEmailValidator>();
            //services.AddTransient<IValidator<UserResetPasswordDto>, UserResetPasswordValidator>();
            //services.AddTransient<IValidator<AdvertisementRequestDto>, AdvertisementValidator>();
            //services.AddTransient<IValidator<AdvertisementUpdateRequestDto>, AdvertisementUpdateValidator>();
            //services.AddTransient<IValidator<CommentRequestDto>, CommentValidator>();
            //services.AddTransient<IValidator<CommentUpdateRequestDto>, CommentUpdateValidator>();


            services.AddScoped<IFileService, FileService>();

            //services.AddSingleton<IMapper>(new Mapper(new MapperConfiguration
            //    (a =>
            //    {
            //        a.AddMaps(Assembly.GetExecutingAssembly());
            //    })));


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
                    ConnectionMultiplexer.Connect("localhost:6379")
                }));
            

            services.AddScoped<ICacheRepository, CacheRepository>();

            services.AddMassTransit(mt => mt.AddMassTransit(x => {
                x.UsingRabbitMq((cntxt, cfg) => {
                    cfg.Host("localhost", "/", c => {
                        c.Username("guest");
                        c.Password("guest");
                    });
                });
            }));

            return services;
        }

        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services/*, IConfiguration configuration*/)
        {
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:7157";
                    //options.ApiName = "Board.Web";
                    options.RequireHttpsMetadata = false;
                    
                });


            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            //    {
            //        var secretKey = configuration["AuthToken:SecretKey"];

            //        options.SaveToken = true;
            //        options.RequireHttpsMetadata = false;
            //        options.TokenValidationParameters = new TokenValidationParameters()
            //        {
            //            ValidateActor = false,
            //            ValidateAudience = false,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            ValidateIssuer = false,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            //        };
            //    });

            return services;
        }

        public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "Board.Web");
                });
            });

            return services;
        }


        //    public static IServiceCollection AddAspNetIdentityServices(this IServiceCollection services)
        //{
        //    services.AddIdentityCore<User>()
        //        .AddRoles<Role>()
        //        .AddUserManager<UserManager<User>>()
        //        .AddRoleManager<RoleManager<Role>>()
        //        .AddEntityFrameworkStores<BoardDbContext>()
        //        .AddDefaultTokenProviders();

        //    services.Configure<IdentityOptions>(options =>
        //    {
        //        options.Password.RequireDigit = true;
        //        options.Password.RequireLowercase = true;
        //        options.Password.RequireNonAlphanumeric = false;
        //        options.Password.RequireUppercase = true;
        //        options.Password.RequiredLength = 8;
        //        options.Password.RequiredUniqueChars = 1;

        //        options.User.AllowedUserNameCharacters =
        //                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
        //        options.User.RequireUniqueEmail = true;

        //        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
        //        options.Lockout.MaxFailedAccessAttempts = 5;
        //        options.Lockout.AllowedForNewUsers = true;

        //        options.SignIn.RequireConfirmedEmail = false;
        //        options.SignIn.RequireConfirmedPhoneNumber = false;
        //    });

        //    return services;
        //}


        public static IServiceCollection AddRedisServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetSection("RedisCache").GetRequiredSection("Host").Value;
                options.InstanceName = configuration.GetSection("RedisCache").GetRequiredSection("InstanceName").Value;
            });

            return services;
        }


        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.FullName.Replace("+", "_"));
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Solarvito Api", Version = "V1" });
                //options.IncludeXmlComments(Path.Combine(Path.Combine(AppContext.BaseDirectory,
                //    $"{typeof(AdvertisementResponseDto).Assembly.GetName().Name}.xml")));
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

        public static IServiceCollection AddHttpClients(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<UserClientOptions>(configuration.GetSection("UserApiClientOptions"));
            services.Configure<FileClientOptions>(configuration.GetSection("FileApiClientOptions"));

            services.AddHttpClient<IUserClient, UserClient>(ConfigureUserHttpClient);
            services.AddHttpClient<IFileClient, FileClient>(ConfigureFileHttpClient);

            return services;
        }


        static void ConfigureUserHttpClient(IServiceProvider serviceProvider, HttpClient client)
        {
            var options = serviceProvider.GetRequiredService<IOptions<UserClientOptions>>().Value;

            client.BaseAddress = new Uri(options.BasePath);
        }

        static void ConfigureFileHttpClient(IServiceProvider serviceProvider, HttpClient client)
        {
            var options = serviceProvider.GetRequiredService<IOptions<FileClientOptions>>().Value;

            client.BaseAddress = new Uri(options.BasePath);
        }

        public static ConfigureHostBuilder AddCustomLogger(this ConfigureHostBuilder hostBuilder, ConfigurationManager configuration)
        {
            hostBuilder.UseSerilog((context, services, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5345"));

            return hostBuilder;
        }

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
