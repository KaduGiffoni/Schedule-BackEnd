using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Schedule.Models; 

namespace Schedule.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Mapeando as nossas classes para virarem tabelas de verdade
        public DbSet<Company> Companies { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Letter> Letters { get; set; }

        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Holiday> Holidays { get; set; }

        public DbSet<UserAbsence> UserAbsences { get; set; }

        public DbSet<SwapRequest> SwapRequests { get; set; }
        public DbSet<ScheduleDay> ScheduleDays { get; set; }

        public DbSet<ShiftPattern> ShiftPatterns { get; set; }

        public DbSet<Notice> Notices { get; set; }
        public DbSet<NoticeAcknowledgment> NoticeByIdAcknowledgments { get; set; }
        public DbSet<NoticeComment> NoticeComments { get; set; }
        public DbSet<Notification> Notifications { get; set; }

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

            
            modelBuilder.Entity<ScheduleDay>()
                .HasIndex(sd => new { sd.LetterId, sd.Date });

           
            modelBuilder.Entity<SwapRequest>()
                .HasIndex(sr => new { sr.TargetUserId, sr.Status });

            modelBuilder.Entity<SwapRequest>()
                .HasIndex(sr => sr.RequestingUserId);

            modelBuilder.Entity<Notice>()
                .HasOne(n => n.CreatedByUser)
                .WithMany()
                .HasForeignKey(n => n.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NoticeAcknowledgment>()
                .HasOne(na => na.User)
                .WithMany()
                .HasForeignKey(na => na.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NoticeComment>()
                .HasOne(nc => nc.User)
                .WithMany()
                .HasForeignKey(nc => nc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.TargetUser)
                .WithMany()
                .HasForeignKey(n => n.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}