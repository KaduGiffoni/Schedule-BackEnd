using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Schedule.Models; 

namespace Schedule.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Mapeando as nossas classes para virarem tabelas de verdade
        public DbSet<Company> Companies { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Letter> Letters { get; set; }

        public DbSet<Shift> Shifts { get; set; }

        public DbSet<SwapRequest> SwapRequests { get; set; }
        public DbSet<ScheduleDay> ScheduleDays { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          
            modelBuilder.Entity<SwapRequest>()
                .HasOne(s => s.RequestingUser)
                .WithMany()
                .HasForeignKey(s => s.RequestingUserId)
                .OnDelete(DeleteBehavior.Restrict); 

           
            modelBuilder.Entity<SwapRequest>()
                .HasOne(s => s.TargetUser)
                .WithMany()
                .HasForeignKey(s => s.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}