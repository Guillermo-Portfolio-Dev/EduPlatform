using MassTransit;
using Notifications.Worker.Consumers; // Asegúrate de que el namespace apunte a donde creaste tu Consumer

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Configuramos MassTransit como un Worker de RabbitMQ
        services.AddMassTransit(x =>
        {
            // Registramos tu consumidor
            x.AddConsumer<StudentRegisteredEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // Cola específica para este worker
                cfg.ReceiveEndpoint("student-registered-queue", e =>
                {
                    e.ConfigureConsumer<StudentRegisteredEventConsumer>(context);
                });
            });
        });
    })
    .Build();

await host.RunAsync();