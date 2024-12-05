using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI_Log.Custom;
using WebAPI_Log.Models;
using WebAPI_Log.Context;
using WebAPI_Log.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuracion de CORS
ConfigureCors(builder.Services);

// Configuracion de JWT
ConfigureJwtAuthentication(builder.Services, builder.Configuration);

// Agregar controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL"))
);

// Servicios adicionales
builder.Services.AddScoped<PerfilService>();
builder.Services.AddSingleton<Utilidades>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Uso de CORS y autenticacion
app.UseCors("AllowVercel");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


// Configuracion de CORS
void ConfigureCors(IServiceCollection services)
{
    services.AddCors(options =>
    {
        options.AddPolicy("AllowVercel", policy =>
        {
            policy.WithOrigins("https://3am-test.vercel.app/") 
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); 
        });
    });
}

// Configuracion de JWT
void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
{
    services.AddAuthentication(config =>
    {
        config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(config =>
    {
        config.RequireHttpsMetadata = builder.Environment.IsDevelopment() ? false : true;
        config.SaveToken = true;
        config.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
        };
    });
}
