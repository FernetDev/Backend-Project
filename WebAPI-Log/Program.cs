using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI_Log.Custom;
using WebAPI_Log.Models;
using WebAPI_Log.Context;
using WebAPI_Log.Services;
using WebAPI_Log.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Configuración de DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL"))
);

// Configuracion de Identity
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedEmail = false;
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole<int>>()
    .AddRoleManager<RoleManager<IdentityRole<int>>>()
    .AddSignInManager<SignInManager<ApplicationUser>>()
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddRoleValidator<RoleValidator<IdentityRole<int>>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configuracion de CORS
ConfigureCors(builder.Services);

// Configuracion de JWT
ConfigureJwtAuthentication(builder.Services, builder.Configuration);

// Agregar controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Servicios adicionales
builder.Services.AddScoped<PerfilService>();
builder.Services.AddScoped<Utilidades>();

var app = builder.Build();

SeedRolesAndUser();

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

// Seed Roles and User
async void SeedRolesAndUser()
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<AppDbContext>();

        //Add Migrations
        db.Database.Migrate();

        try
        {
            //Add Roles, if they don't exist yet
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
            if (!roleManager.RoleExistsAsync("ADMIN").Result)
            {
                var role = new IdentityRole<int>();
                role.Name = "ADMIN";
                roleManager.CreateAsync(role).Wait();
            }
            if (!roleManager.RoleExistsAsync("MANAGER").Result)
            {
                var role = new IdentityRole<int>();
                role.Name = "MANAGER";
                roleManager.CreateAsync(role).Wait();
            }
            if (!roleManager.RoleExistsAsync("COACH").Result)
            {
                var role = new IdentityRole<int>();
                role.Name = "COACH";
                roleManager.CreateAsync(role).Wait();
            }
            if (!roleManager.RoleExistsAsync("MEMBER").Result)
            {
                var role = new IdentityRole<int>();
                role.Name = "MEMBER";
                roleManager.CreateAsync(role).Wait();
            }

            //Add Admin User, if it doesn't exist yet
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            if (!userManager.Users.Any(x => x.UserName.ToLower() == "admin"))
            {
                var user = new ApplicationUser
                {
                    Nombre = "admin",
                    Email = "admin@sinnombredevs.com",
                    UserName = "admin",
                    Clave = "Asdf1234."
                };

                await userManager.CreateAsync(user, "Asdf1234.");
                await userManager.AddToRoleAsync(user, "ADMIN");
            }

            //Call seed user method
            //await SeedUsers.SeedUsersAsync(userManager, roleManager);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}