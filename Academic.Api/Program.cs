using Academic.Application.Students.Commands.RegisterStudent;
using Academic.Domain.Repositories;
using Academic.Infrastructure.Persistence;
using Academic.Infrastructure.Repositories;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configurar DbConnectionFactory para Dapper
var connectionString = builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException("La conexion Databse no existe.");

builder.Services.AddSingleton(new DbConnectionFactory(connectionString));
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

// Configurar MassTransit con RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

// Registrar MediatR (apuntando al ensamblado de la capa Application)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterStudentCommand).Assembly));

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.MapScalarApiReference();
//}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();