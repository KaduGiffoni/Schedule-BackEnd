using Microsoft.AspNetCore.Identity;
using Schedule.Models;

namespace Schedule.Data
{
    public static class DbSeeder
    {

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