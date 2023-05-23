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
using Minio;
using FileStorage.Contracts;
using Microsoft.Extensions.Options;

namespace FileStorage.Infrastructure.Registrar
{
    public static class FileStorageRegistrar
    {
        public static IServiceCollection AddServiceRegistrationModule(this IServiceCollection services)
        {
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IImageService, ImageService>();

            
            services.AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()));
            services.AddValidatorsFromAssembly(typeof(ImageUploadValidator).Assembly);


            services.AddScoped<IObjectStorage, MinioStorage>();
            services.AddScoped<MinioClientConfiguration>();
            services.AddOptions<MinioClientOptions>()
                .BindConfiguration("MinioServer")
                .ValidateOnStart();
            services.AddScoped<MinioClient>(services =>
            {
                return services.GetRequiredService<MinioClientConfiguration>().Configure();
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
