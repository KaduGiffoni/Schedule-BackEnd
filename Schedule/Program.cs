using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Schedule.Data;
using Schedule.Models;
using Schedule.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Adiciona os serviços para Controladores e Swagger
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Ensina o tradutor a ignorar o loop infinito das chaves estrangeiras
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<SwapRequestService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Isso habilita a geração do Swagger

// Configura o Entity Framework com SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configura o Identity para gerenciar usuários e login
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// 2. Configura o Pipeline de requisições (Middleware)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Gera o arquivo JSON
    app.UseSwaggerUI(); // Cria a interface visual bonitinha
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapIdentityApi<ApplicationUser>(); // Isso cria as rotas de Registro e Login automaticamente!

app.MapControllers();

app.Run();