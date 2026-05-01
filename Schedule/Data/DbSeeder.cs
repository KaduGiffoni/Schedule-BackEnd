using Microsoft.AspNetCore.Identity;

namespace Schedule.Data
{ 
    public static class DbSeeder
    {
       
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "Coordinator", "Standard","Viewer" };

            foreach (var roleName in roleNames)
            {
                
                var roleExist = await roleManager.RoleExistsAsync(roleName);

               
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}