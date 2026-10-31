using Billing.Worker.Consumers;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StudentRegisteredBillingConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // Nombre de cola independiente para el microservicio de billing
        cfg.ReceiveEndpoint("billing-student-registered-queue", e =>
        {
            e.ConfigureConsumer<StudentRegisteredBillingConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();