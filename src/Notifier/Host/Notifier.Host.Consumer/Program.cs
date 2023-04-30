using MassTransit;
using Notifier.Application.AppData.Contexts.Messages.Services;
using Notifier.Host.Consumer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IMessageService, MessageService>();

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
