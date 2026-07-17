using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Schedule.Models;
using Schedule.Models.KnowledgeBase;
using Schedule.Models.KnowledgeBase.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Schedule.Data;

public static class DbSeeder
{
    // ========================================================
    // 1. SEEDING DA BASE DE CONHECIMENTO (Categorias e Artigo)
    // ========================================================
    public static async Task SeedKnowledgeBase(ApplicationDbContext context)
    {
        // Se já existirem categorias, ignora o seeding para evitar duplicados.
        if (await context.KnowledgeCategories.AnyAsync()) return;

        // VERIFICAÇÃO DE INTEGRIDADE (Impede erro de FK constraint)
        var author = await context.Users.FirstOrDefaultAsync();
        if (author == null)
        {
            // Se não houver utilizadores na base de dados, não podemos associar artigos.
            // O ideal é que o 'SeedFirstAdminAsync' rode primeiro na pipeline.
            return;
        }

        var categoryRedes = new KnowledgeCategory
        {
            Name = "Redes e Infraestrutura",
            Slug = "redes-e-infraestrutura",
            Description = "Procedimentos operacionais padronizados para roteamento, switching e troubleshooting."
        };

        await context.KnowledgeCategories.AddAsync(categoryRedes);

        // Uso de coleção para inserção em lote (Bulk Insert) mais otimizada
        var tags = new[]
        {
            new KnowledgeTag { Name = "IPv4", Slug = "ipv4" },
            new KnowledgeTag { Name = "Subnet", Slug = "subnet" },
            new KnowledgeTag { Name = "Cisco", Slug = "cisco" },
            new KnowledgeTag { Name = "Firewall", Slug = "firewall" }
        };

        await context.KnowledgeTags.AddRangeAsync(tags);

        // Devemos guardar primeiro para gerar os Ids das categorias e tags
        await context.SaveChangesAsync();

        var articleId = Guid.NewGuid();
        var initialVersion = new KnowledgeArticleVersion
        {
            ArticleId = articleId,
            Title = "Como calcular sub-redes IPv4",
            Summary = "Guia prático para cálculo rápido de sub-redes.",
            Content = "## Passo 1\nIdentifique a máscara de rede...\n\n## Passo 2\nCalcule os bits emprestados...",
            EstimatedTimeInMinutes = 15,
            Difficulty = DifficultyLevel.Intermediate,
            EditorId = author.Id,
            VersionNumber = 1,
            ChangeDescription = "Versão inicial (Seeder)"
        };

        var article = new KnowledgeArticle
        {
            Id = articleId,
            CategoryId = categoryRedes.Id,
            AuthorId = author.Id,
            Status = ArticleStatus.Published,
            Slug = "como-calcular-sub-redes-ipv4",
            CurrentVersionId = initialVersion.Id,
            CreatedAt = DateTime.UtcNow
        };

        article.Versions.Add(initialVersion);

        // Associar as tags através da entidade de ligação
        foreach (var tag in tags)
        {
            article.ArticleTags.Add(new KnowledgeArticleTag { ArticleId = articleId, TagId = tag.Id });
        }

        await context.KnowledgeArticles.AddAsync(article);
        await context.SaveChangesAsync();
    }

    // ========================================================
    // 2. SEEDING DE ROLES DE ACESSO
    // ========================================================
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = { "Admin", "Manager", "Standard", "Viewer", "Editor", "Administrator" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    // ========================================================
    // 3. SEEDING DO PRIMEIRO ADMINISTRADOR
    // ========================================================
    public static async Task SeedFirstAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var existingAdmins = await userManager.GetUsersInRoleAsync("Admin");
        if (existingAdmins.Any())
            return;

        var bootstrapEmail = configuration["Bootstrap:AdminEmail"];
        if (string.IsNullOrWhiteSpace(bootstrapEmail))
            return;

        var user = await userManager.FindByEmailAsync(bootstrapEmail);
        if (user == null)
            return;

        // O Identity garante que não duplica se já tiver a role
        await userManager.AddToRoleAsync(user, "Admin");
        await userManager.AddToRoleAsync(user, "Administrator");
    }
}