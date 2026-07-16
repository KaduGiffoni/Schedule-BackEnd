using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Quartz;
using Schedule.Data;
using Schedule.Interfaces.KnowledgeBase;
using Schedule.Jobs;
using Schedule.Models;
using Schedule.Repositories.KnowledgeBase;
using Schedule.Services;
using Schedule.Services.KnowledgeBase;
using Serilog;
using System.Reflection;


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
builder.Services.AddHttpClient<HolidayService>();
builder.Services.AddScoped<HolidayService>();
builder.Services.AddScoped<NoticeService>();
builder.Services.AddScoped<AbsenceService>();

// Knowledge Base — Repositórios e Serviço
builder.Services.AddScoped<IKnowledgeArticleRepository,  KnowledgeArticleRepository>();
builder.Services.AddScoped<IKnowledgeCategoryRepository, KnowledgeCategoryRepository>();
builder.Services.AddScoped<IKnowledgeTagRepository,      KnowledgeTagRepository>();
builder.Services.AddScoped<IKnowledgeBaseService,        KnowledgeBaseService>();

//automapper + fluentvalidation
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<Schedule.Mapping.KnowledgeBaseProfile>();
});
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

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

// ==========================================
// CORREÇÃO 1: O Motor Automático com Roles
// ==========================================
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
//job
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("SyncHolidaysJob");
    q.AddJob<SyncHolidaysJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("JanFirstTrigger")
        .WithCronSchedule("0 0 0 1 1 ?"));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("MonthlyCheckTrigger")
        .WithCronSchedule("0 0 3 1 * ?"));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        await DbSeeder.SeedRolesAsync(services);
        await DbSeeder.SeedFirstAdminAsync(services, app.Configuration);
        await DbSeeder.SeedKnowledgeBase(context);
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Aviso: O banco de dados não está pronto ou as tabelas ainda não existem. O Seeding foi ignorado nesta execução.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PermitirFrontEnd");

app.UseAuthentication();
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
