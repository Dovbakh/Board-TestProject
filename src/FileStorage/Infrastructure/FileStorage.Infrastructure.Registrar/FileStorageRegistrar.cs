﻿using AutoMapper;
using FileStorage.Application.AppData.Contexts.Files.Repositories;
using FileStorage.Application.AppData.Contexts.Files.Services;
using FileStorage.Infrastructure.DataAccess.Contexts.Files.Repositories;
using FileStorage.Infrastructure.ObjectStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Infrastructure.Registrar
{
    public static class FileStorageRegistrar
    {
        public static IServiceCollection AddServiceRegistrationModule(this IServiceCollection services)
        {
            services.AddScoped<IFileRepository, FileRepository>();

            services.AddScoped<IFileService, FileService>();

            services.AddScoped<IObjectStorage, MinioStorage>();

            services.AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()));

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
