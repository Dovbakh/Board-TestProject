using MassTransit;
using Notifier.Application.AppData.Contexts.Messages.Services;
using Notifier.Host.Consumer;
using Serilog;
using Notifier.Infrastructure.Registrar;
using Notifier.Contracts.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(builder.Configuration["Seq:Address"]));

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddMassTransit(mt => mt.AddMassTransit(x => {

    x.AddConsumer<MassTransitConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Address"], "/", c =>
        {
            c.Username(builder.Configuration["RabbitMQ:Username"]);
            c.Password(builder.Configuration["RabbitMQ:Password"]);
        });
        cfg.ConfigureEndpoints(context);

    });
}));

builder.Services.AddOptions<SmtpOptions>()
    .BindConfiguration("SmtpServer")
    .ValidateOnStart();




var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
