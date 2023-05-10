using AutoMapper;
using FileStorage.Application.AppData.Contexts.Images.Repositories;
using FileStorage.Application.AppData.Contexts.Images.Services;
using FileStorage.Infrastructure.DataAccess.Contexts.Images.Repositories;
using FileStorage.Infrastructure.ObjectStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Identity.Application.AppData.Contexts.Images.Helpers;

namespace FileStorage.Infrastructure.Registrar
{
    public static class FileStorageRegistrar
    {
        public static IServiceCollection AddServiceRegistrationModule(this IServiceCollection services)
        {
            services.AddScoped<IImageRepository, ImageRepository>();

            services.AddScoped<IImageService, ImageService>();

            services.AddScoped<IObjectStorage, MinioStorage>();

            services.AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()));

            services.AddValidatorsFromAssembly(typeof(ImageUploadValidator).Assembly);

            return services;
        }

        //public static ConfigureHostBuilder AddCustomLogger(this ConfigureHostBuilder hostBuilder, ConfigurationManager configuration)
        //{
        //    hostBuilder.UseSerilog((context, services, configuration) =>
        //        configuration.ReadFrom.Configuration(context.Configuration)
        //        .Enrich.FromLogContext()
        //        .WriteTo.Console()
        //        .WriteTo.Seq("http://localhost:5345"));

        //    return hostBuilder;
        //}

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
