using AutoMapper;
using FileStorage.Application.AppData.Contexts.Files.Repositories;
using FileStorage.Application.AppData.Contexts.Files.Services;
using FileStorage.Infrastructure.DataAccess.Contexts.Files.Repositories;
using FileStorage.Infrastructure.ObjectStorage;
using Microsoft.Extensions.DependencyInjection;
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
