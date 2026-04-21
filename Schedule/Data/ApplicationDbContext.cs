using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Schedule.Models; // Avisa onde estão as nossas classes

namespace Schedule.Data
{
    // Agora o banco sabe que vai usar a nossa classe ApplicationUser customizada
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Mapeando as nossas classes para virarem tabelas de verdade
        public DbSet<Company> Companies { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Letter> Letters { get; set; }
    }
}