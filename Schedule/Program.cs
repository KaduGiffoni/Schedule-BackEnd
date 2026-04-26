using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
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

builder.Services.AddSwaggerGen(c =>
{
    // 1. Criando a "Fechadura"
    c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Insira APENAS o seu token JWT abaixo."
    });

    // 2. Avisando ao Swagger para colocar o cadeado (NOVO PADRÃO .NET 10)
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("bearer", document),
            new List<string>() // <-- A CORREÇÃO ESTÁ AQUI! Trocamos o Switch físico pelo Virtual.
        }
    });
});

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