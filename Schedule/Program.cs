using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // <-- ESTA É A LINHA QUE FALTAVA!
using Schedule.Data;
using Schedule.Models;
using Schedule.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ======================
// LOGGING (Serilog)
// ======================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/escala_log_.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ======================
// SERVICES
// ======================
// Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Application Services
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<SwapRequestService>();

// Swagger (Corrigido para .NET 8 / Swashbuckle 6.6.2)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Insira APENAS o seu token JWT abaixo."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================================
// CORREÇÃO 1: O Motor Automático com Roles
// ==========================================
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontEnd", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ======================
// APP & MIDDLEWARE
// ======================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PermitirFrontEnd");

// ==========================================
// CORREÇÃO 2: A ordem exata da Segurança
// ==========================================
app.UseAuthentication(); // 1º Lê o Token (Quem é você?)
app.UseAuthorization();  // 2º Verifica as Roles (O que você pode fazer?)

// ======================
// ENDPOINTS
// ======================
app.MapIdentityApi<ApplicationUser>();
app.MapControllers();

// ======================
// RUN
// ======================
try
{
    Log.Information("A iniciar a API de Escalas...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A API falhou de forma catastrófica durante o arranque.");
}
finally
{
    Log.CloseAndFlush();
}