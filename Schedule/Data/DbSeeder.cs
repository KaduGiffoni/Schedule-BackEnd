using Microsoft.AspNetCore.Identity;
using Schedule.Models;
using Schedule.Models.KnowledgeBase;

namespace Schedule.Data
{
    public static class DbSeeder
    {
        public static async Task SeedKnowledgeBase(ApplicationDbContext context)
        {
            if (await context.KnowledgeCategories.AnyAsync()) return;

            // 1. Criar Categorias Raiz
            var infra = new KnowledgeCategory { Name = "Infraestrutura", Slug = "infraestrutura" };
            var telco = new KnowledgeCategory { Name = "Telefonia", Slug = "telefonia" };

            context.KnowledgeCategories.AddRange(infra, telco);
            await context.SaveChangesAsync();

            // 2. Criar Subcategoria
            var cucm = new KnowledgeCategory { Name = "CUCM", Slug = "cucm", ParentCategoryId = telco.Id };
            context.KnowledgeCategories.Add(cucm);
            await context.SaveChangesAsync();

            // 3. Criar Selo de Gamificação (RB033)
            var badge = new KnowledgeBadge
            {
                Name = "Especialista em Telefonia",
                Description = "Concluiu todos os procedimentos de Telefonia.",
                CategoryId = telco.Id,
                ImageUrl = "/assets/badges/telco-gold.png"
            };
            context.KnowledgeBadges.Add(badge);
            await context.SaveChangesAsync();
        }


        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "Manager", "Standard", "Viewer" };

            foreach (var roleName in roleNames)
            {

                var roleExist = await roleManager.RoleExistsAsync(roleName);


                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        // Resolve o impasse "ovo e galinha": o endpoint /api/Auth/user-promoter só pode
        // ser chamado por um Admin, mas sem essa rotina NENHUM usuário nasce Admin.
        // Se ainda não existir nenhum Admin no sistema, promove o e-mail configurado em
        // appsettings.json -> "Bootstrap:AdminEmail". Depois que o primeiro Admin existir,
        // essa rotina nunca mais faz nada (idempotente e segura em todo restart da API).
        public static async Task SeedFirstAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var existingAdmins = await userManager.GetUsersInRoleAsync("Admin");
            if (existingAdmins.Any())
                return; // Já existe pelo menos um Admin, não faz nada.

            var bootstrapEmail = configuration["Bootstrap:AdminEmail"];
            if (string.IsNullOrWhiteSpace(bootstrapEmail))
                return; // Ninguém configurou um e-mail de bootstrap, não faz nada.

            var user = await userManager.FindByEmailAsync(bootstrapEmail);
            if (user == null)
                return; // O e-mail configurado ainda não tem conta criada.

            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}