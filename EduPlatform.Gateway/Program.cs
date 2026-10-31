using EduPlatform.Gateway.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar la autenticación JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? "SuperSecretKeyForEduPlatformSecurity2026SecureKey!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "EduPlatform.Auth",
            ValidAudience = jwtSettings["Audience"] ?? "EduPlatform.Clients",            
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Política por defecto para restringir acceso sin token
    //options.AddPolicy("DefaultPolicy", policy =>
    //    policy.RequireAuthenticatedUser());

    // Política exclusiva para administradores
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // Política para usuarios con rol Student o Admin
    options.AddPolicy("AcademicAccess", policy =>
        policy.RequireRole("Admin", "Student"));
});

builder.Services.AddScoped<IUserRepository, UserRepository>();

// 1. Agregar soporte para controladores (para nuestro endpoint de Login)
builder.Services.AddControllers();


// Carga la configuración de YARP desde appsettings.json
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "EduPlatform Gateway is running!");

//habilitar las rutas del proxy
app.MapReverseProxy();

app.Run();
