using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
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

// Swagger
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

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("bearer", document),
            new List<string>()
        }
    });
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
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
// APP
// ======================
var app = builder.Build();

// ======================
// MIDDLEWARE
// ======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("PermitirFrontEnd");

app.UseAuthorization();

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