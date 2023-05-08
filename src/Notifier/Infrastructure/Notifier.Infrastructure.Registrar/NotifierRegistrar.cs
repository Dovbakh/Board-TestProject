using Microsoft.Extensions.DependencyInjection;
using Notifier.Application.AppData.Contexts.Messages.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifier.Infrastructure.Registrar
{
    public static class NotifierRegistrar
    {
        public static IServiceCollection AddServiceRegistrationModule (this IServiceCollection services)
        {
            services.AddScoped<IMessageService, MessageService>();

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
    }
}
