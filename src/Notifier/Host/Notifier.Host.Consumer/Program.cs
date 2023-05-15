using MassTransit;
using Notifier.Application.AppData.Contexts.Messages.Services;
using Notifier.Host.Consumer;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5345"));

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddMassTransit(mt => mt.AddMassTransit(x => {

    x.AddConsumer<MassTransitConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", c =>
        {
            c.Username("guest");
            c.Password("guest");
        });
        cfg.ConfigureEndpoints(context);

    });
    }));



var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
