using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Schedule.Models;
using Schedule.Models.KnowledgeBase;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.Data
{
    public static class DbSeeder
    {
        // ========================================================
        // 1. SEEDING DA BASE DE CONHECIMENTO (Categorias e Artigo)
        // ========================================================
        public static async Task SeedKnowledgeBase(ApplicationDbContext context)
        {
            if (context.KnowledgeCategories.Any()) return;

            var author = await context.Users.FirstOrDefaultAsync();
            string authorId = author?.Id ?? Guid.NewGuid().ToString();

            var categoryRedes = new KnowledgeCategory
            {
                Name = "Redes e Infraestrutura",
                Slug = "redes-e-infraestrutura",
                Description = "Procedimentos operacionais padronizados para roteamento, switching e troubleshooting."
            };
            context.KnowledgeCategories.Add(categoryRedes);

            var tagIp = new KnowledgeTag { Name = "IPv4", Slug = "ipv4" };
            var tagSubnet = new KnowledgeTag { Name = "Subnet", Slug = "subnet" };
            var tagCisco = new KnowledgeTag { Name = "Cisco", Slug = "cisco" };
            context.KnowledgeTags.AddRange(tagIp, tagSubnet, tagCisco);

            await context.SaveChangesAsync();

            var article = new KnowledgeArticle
            {
                AuthorId = authorId,
                CategoryId = categoryRedes.Id,
                Status = ArticleStatus.Published,
                Slug = "como-calcular-mascara-de-sub-rede-ip"
            };

            var version = new KnowledgeArticleVersion
            {
                Title = "Como calcular uma Máscara de Sub-rede (IPv4)",
                Summary = "Aprenda de forma prática como calcular máscaras de sub-rede IPv4, descobrindo o endereço de rede, broadcast e quantidade de IPs válidos.",
                Content = @"### O que é uma Máscara de Sub-rede?

A máscara de sub-rede divide um endereço IP em duas partes lógicas: a **identificação da rede** e a **identificação do host**. É ela que diz ao roteador onde uma rede começa e onde termina.

### Exemplo Prático: Calculando um `/24`

Para calcular a capacidade de uma rede, precisamos olhar para os bits.
1. **Identifique a notação CIDR:** O `/24` significa que os primeiros 24 bits estão 'ligados' (1).
2. **Converta para decimal:** `255.255.255.0`.
3. **Calcule os hosts válidos:** Faltam 8 bits para completar os 32 bits totais do IPv4. 
A fórmula de hosts é `2^n - 2` (onde 'n' são os bits restantes).
Portanto: `2^8 - 2` = **254 hosts válidos**.

> **Dica do NOC:** Em cenários de troubleshooting rápido ou configuração de túneis P2P (onde normalmente usamos `/30`), certifique-se de validar se o gateway está no range útil usando uma calculadora VLSM para evitar sobreposição em produção!
",
                EstimatedTimeInMinutes = 5,
                Difficulty = DifficultyLevel.Intermediate,
                EditorId = authorId,
                VersionNumber = 1,
                ChangeDescription = "Publicação inicial gerada automaticamente pelo sistema (Onboarding)."
            };

            article.Versions.Add(version);

            article.ArticleTags.Add(new KnowledgeArticleTag { TagId = tagIp.Id });
            article.ArticleTags.Add(new KnowledgeArticleTag { TagId = tagSubnet.Id });

            context.KnowledgeArticles.Add(article);
            await context.SaveChangesAsync();

            article.CurrentVersionId = version.Id;
            context.KnowledgeArticles.Update(article);

            var badge = new KnowledgeBadge
            {
                Name = "Especialista em Redes",
                Description = "Concluiu a leitura de todos os procedimentos de Redes e Infraestrutura.",
                CategoryId = categoryRedes.Id,
                ImageUrl = "/assets/badges/network-gold.png"
            };
            context.KnowledgeBadges.Add(badge);

            await context.SaveChangesAsync();
        }

        // ========================================================
        // 2. SEEDING DE CARGOS E PERMISSÕES
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

            await userManager.AddToRoleAsync(user, "Admin");
            await userManager.AddToRoleAsync(user, "Administrator");
        }
    }
}